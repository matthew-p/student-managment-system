using Microsoft.EntityFrameworkCore;

namespace StudentManagment.Models
{
    public class StudentManagmentContext : DbContext
    {
        public StudentManagmentContext(DbContextOptions<StudentManagmentContext> options) 
            : base(options)
            {
            }

            public DbSet<Student> Students { get; set; }
    }
}