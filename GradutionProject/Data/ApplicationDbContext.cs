using GradutionProject.Abstractions;
using GradutionProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace GradutionProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<NewStudent> NewStudents { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Cours> Courses { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<GradeOfStudent> GradeOfStudents { get; set; }
        public DbSet<GradeAppeal> GradeAppeals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureEntity();
            modelBuilder.Entity<Admin>().HasData(
    new Admin
    {
        Id = 1,
        Name = "Admin",
        Email = "Admin@Admin.com",
        Password = "$2a$11$9pZSxyMXmpfXIX9oGamqXONBpqXuOVHpqB9tDTbuwao786IZvpa1q",
        Phone = "999",
        CreatedDate = new DateTime(2024, 01, 01),
        LastModifiedDate = new DateTime(2024, 01, 01)
    }
);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<College>()
           .HasMany(c => c.Students)
           .WithOne(s => s.College)
           .HasForeignKey(s => s.CollegeId)
           .OnDelete(DeleteBehavior.NoAction);
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<Entity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.LastModifiedDate = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedDate = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }

    }
}
