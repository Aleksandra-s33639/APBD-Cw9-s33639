using APBD_Cw9_s33639.Data;
using APBD_Cw9_s33639.DTOs;
using APBD_Cw9_s33639.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD_Cw9_s33639.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public PatientsController(HospitalDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatients([FromQuery] string? search)
    {
        var query = _context.Patients
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";

            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, pattern) ||
                EF.Functions.Like(p.LastName, pattern));
        }

        var patients = await query
            .Select(p => new PatientDto
            {
                Pesel = p.Pesel,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age,
                Sex = p.Sex ? "Male" : "Female",

                Admissions = p.Admissions.Select(a => new AdmissionDto
                {
                    Id = a.Id,
                    AdmissionDate = a.AdmissionDate,
                    DischargeDate = a.DischargeDate,
                    Ward = new WardDto
                    {
                        Id = a.Ward.Id,
                        Name = a.Ward.Name,
                        Description = a.Ward.Description
                    }
                }).ToList(),

                BedAssignments = p.BedAssignments.Select(ba => new BedAssignmentDto
                {
                    Id = ba.Id,
                    From = ba.From,
                    To = ba.To,
                    Bed = new BedDto
                    {
                        Id = ba.Bed.Id,
                        BedType = new BedTypeDto
                        {
                            Id = ba.Bed.BedType.Id,
                            Name = ba.Bed.BedType.Name,
                            Description = ba.Bed.BedType.Description
                        },
                        Room = new RoomDto
                        {
                            Id = ba.Bed.Room.Id,
                            HasTv = ba.Bed.Room.HasTv,
                            Ward = new WardDto
                            {
                                Id = ba.Bed.Room.Ward.Id,
                                Name = ba.Bed.Room.Ward.Name,
                                Description = ba.Bed.Room.Ward.Description
                            }
                        }
                    }
                }).ToList()
            })
            .ToListAsync();

        return Ok(patients);
    }
    
    [HttpPost("{pesel}/bedassignments")]
public async Task<IActionResult> CreateBedAssignment(
    string pesel,
    [FromBody] CreateBedAssignmentRequest request)
{
    if (request.To.HasValue && request.To <= request.From)
    {
        return BadRequest(new
        {
            message = "Data zakończenia przypisania łóżka musi być późniejsza niż data rozpoczęcia."
        });
    }

    var patientExists = await _context.Patients.AnyAsync(p => p.Pesel == pesel);

    if (!patientExists)
    {
        return NotFound(new
        {
            message = $"Nie znaleziono pacjenta o numerze PESEL {pesel}."
        });
    }

    var bedTypeExists = await _context.BedTypes.AnyAsync(bt => bt.Name == request.BedType);

    if (!bedTypeExists)
    {
        return NotFound(new
        {
            message = $"Nie znaleziono typu łóżka: {request.BedType}."
        });
    }

    var wardExists = await _context.Wards.AnyAsync(w => w.Name == request.Ward);

    if (!wardExists)
    {
        return NotFound(new
        {
            message = $"Nie znaleziono oddziału: {request.Ward}."
        });
    }

    var requestedTo = request.To ?? DateTime.MaxValue;

    var bedQuery = _context.Beds
        .Include(b => b.BedType)
        .Include(b => b.Room)
        .ThenInclude(r => r.Ward)
        .Where(b =>
            b.BedType.Name == request.BedType &&
            b.Room.Ward.Name == request.Ward);

    if (request.To.HasValue)
    {
        bedQuery = bedQuery.Where(b => !b.BedAssignments.Any(ba =>
            ba.From < request.To.Value &&
            (ba.To == null || ba.To > request.From)));
    }
    else
    {
        bedQuery = bedQuery.Where(b => !b.BedAssignments.Any(ba =>
            ba.To == null || ba.To > request.From));
    }

    var bed = await bedQuery.FirstOrDefaultAsync();

    if (bed is null)
    {
        return NotFound(new
        {
            message = "Nie znaleziono wolnego łóżka spełniającego podane kryteria w wybranym okresie."
        });
    }

    var assignment = new BedAssignment
    {
        PatientPesel = pesel,
        BedId = bed.Id,
        From = request.From,
        To = request.To
    };

    _context.BedAssignments.Add(assignment);
    await _context.SaveChangesAsync();

    return Created($"/api/patients/{pesel}/bedassignments/{assignment.Id}", new
    {
        id = assignment.Id,
        patientPesel = assignment.PatientPesel,
        bedId = assignment.BedId,
        from = assignment.From,
        to = assignment.To
    });
}
}