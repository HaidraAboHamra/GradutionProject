using GradutionProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace GradutionProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Professsor> Professsors { get; set; }
        public DbSet<Cours> Courses { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(
                new Admin { Id = 1, Name = "Admin", Email = "Admin@Admin.com", Password = "AQAAAAIAAYagAAAAEORnOyHZWpGTFS206rXM8pdrBz/Y6pJVOVO8gnGRg6hlLw0VLtacH0ZIGx5Rk9/a0A==", Phone = "999" }
                );
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<College>()
           .HasMany(c => c.Students)
           .WithOne(s => s.College)
           .HasForeignKey(s => s.CollegeId)
           .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
