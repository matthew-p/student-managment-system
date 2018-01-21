using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Models
{
    public interface IStudentManagementContext
    {
        DbSet<Student> Students { get; set; }
    }

    public class StudentManagementContext : DbContext, IStudentManagementContext
    {
        public StudentManagementContext(DbContextOptions<StudentManagementContext> options) 
            : base(options)
            {
            }

            public DbSet<Student> Students { get; set; }
    }
}