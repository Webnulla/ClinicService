using Microsoft.EntityFrameworkCore;

namespace ClinicService.Data;

public class ClinicServiceDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Consultation> Consultations { get; set; }
    
    //переопределяем метод, чтобы бд смогла сделать миграцию и апдейт без ошибок
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Consultation>()
            .HasOne(p => p.Pet)
            .WithMany(c => c.Consultations)
            .HasForeignKey(p => p.PetId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    public ClinicServiceDbContext(DbContextOptions options) : base(options)
    {
    }
}