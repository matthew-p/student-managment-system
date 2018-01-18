using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Models
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