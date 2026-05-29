using System;
using System.Collections.Generic;
using APBD_Cw9_s33639.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_Cw9_s33639.Data;

public partial class HospitalDbContext : DbContext
{
    public HospitalDbContext()
    {
    }

    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admission> Admissions { get; set; }

    public virtual DbSet<Bed> Beds { get; set; }

    public virtual DbSet<BedAssignment> BedAssignments { get; set; }

    public virtual DbSet<BedType> BedTypes { get; set; }

    public virtual DbSet<Component> Components { get; set; }

    public virtual DbSet<ComponentManufacturer> ComponentManufacturers { get; set; }

    public virtual DbSet<ComponentType> ComponentTypes { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Pc> Pcs { get; set; }

    public virtual DbSet<Pccomponent> Pccomponents { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Ward> Wards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS01;Database=APBD2;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Admissions_pk");

            entity.Property(e => e.AdmissionDate).HasColumnType("datetime");
            entity.Property(e => e.DischargeDate).HasColumnType("datetime");
            entity.Property(e => e.PatientPesel)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.PatientPeselNavigation).WithMany(p => p.Admissions)
                .HasForeignKey(d => d.PatientPesel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Admissions_Patients");

            entity.HasOne(d => d.Ward).WithMany(p => p.Admissions)
                .HasForeignKey(d => d.WardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Admissions_Wards");
        });

        modelBuilder.Entity<Bed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Beds_pk");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.RoomId)
                .HasMaxLength(4)
                .IsUnicode(false);

            entity.HasOne(d => d.BedType).WithMany(p => p.Beds)
                .HasForeignKey(d => d.BedTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Beds_BedTypes");

            entity.HasOne(d => d.Room).WithMany(p => p.Beds)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Beds_Rooms");
        });

        modelBuilder.Entity<BedAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BedAssignments_pk");

            entity.Property(e => e.From).HasColumnType("datetime");
            entity.Property(e => e.PatientPesel)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.To).HasColumnType("datetime");

            entity.HasOne(d => d.Bed).WithMany(p => p.BedAssignments)
                .HasForeignKey(d => d.BedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BedAssignments_Beds");

            entity.HasOne(d => d.PatientPeselNavigation).WithMany(p => p.BedAssignments)
                .HasForeignKey(d => d.PatientPesel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BedAssignments_Patients");
        });

        modelBuilder.Entity<BedType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BedTypes_pk");

            entity.Property(e => e.Name).HasMaxLength(300);
        });

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.HasIndex(e => e.ComponentManufacturerId, "IX_Components_ComponentManufacturerId");

            entity.HasIndex(e => e.ComponentTypeId, "IX_Components_ComponentTypeId");

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(300);

            entity.HasOne(d => d.ComponentManufacturer).WithMany(p => p.Components).HasForeignKey(d => d.ComponentManufacturerId);

            entity.HasOne(d => d.ComponentType).WithMany(p => p.Components).HasForeignKey(d => d.ComponentTypeId);
        });

        modelBuilder.Entity<ComponentManufacturer>(entity =>
        {
            entity.Property(e => e.Abbreviation).HasMaxLength(30);
            entity.Property(e => e.FullName).HasMaxLength(300);
        });

        modelBuilder.Entity<ComponentType>(entity =>
        {
            entity.Property(e => e.Abbreviation).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Pesel).HasName("Patients_pk");

            entity.Property(e => e.Pesel)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<Pc>(entity =>
        {
            entity.ToTable("PCs");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Pccomponent>(entity =>
        {
            entity.HasKey(e => new { e.PcId, e.ComponentCode });

            entity.ToTable("PCComponents");

            entity.HasIndex(e => e.ComponentCode, "IX_PCComponents_ComponentCode");

            entity.Property(e => e.ComponentCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.ComponentCodeNavigation).WithMany(p => p.Pccomponents).HasForeignKey(d => d.ComponentCode);

            entity.HasOne(d => d.Pc).WithMany(p => p.Pccomponents).HasForeignKey(d => d.PcId);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Rooms_pk");

            entity.Property(e => e.Id)
                .HasMaxLength(4)
                .IsUnicode(false);

            entity.HasOne(d => d.Ward).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.WardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Room_Ward");
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Wards_pk");

            entity.Property(e => e.Name).HasMaxLength(300);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
