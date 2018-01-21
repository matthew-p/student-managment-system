using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace StudentManagement.Models
{
    public interface IStudentManagementContext
    {
        DbSet<Student> Students { get; set; }
    }

    public class StudentManagmentContext : DbContext, IStudentManagementContext
    {
        public StudentManagmentContext(DbContextOptions<StudentManagmentContext> options) 
            : base(options)
            {
            }

            public DbSet<Student> Students { get; set; }
    }
}