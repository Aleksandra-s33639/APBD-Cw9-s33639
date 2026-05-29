using System.ComponentModel.DataAnnotations;

namespace APBD_Cw9_s33639.DTOs;

public class CreateBedAssignmentRequest
{
    [Required]
    public DateTime From { get; set; }

    public DateTime? To { get; set; }

    [Required]
    public string BedType { get; set; } = null!;

    [Required]
    public string Ward { get; set; } = null!;
}