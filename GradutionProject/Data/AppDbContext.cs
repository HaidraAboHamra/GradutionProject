using GradutionProject.Entity;
using Microsoft.EntityFrameworkCore;

namespace GradutionProject.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option){}

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Admin", Email = "Admin@haidra.com", IsAdmin = true, PasswordHash = "AQAAAAIAAYagAAAAEJghltB0BNLKhi4+ryYg4P6b5tD//sNikG39lK2dTZz/xNNzkKhNTyP+cFKP5O0Rcw==", PhoneNumber = "99" ,Claim ="hi" }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
