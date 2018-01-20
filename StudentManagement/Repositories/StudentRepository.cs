using System;
using System.Collections.Generic;
using System.Linq;
using StudentManagement.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace StudentManagement.Repositories
{
    public interface IStudentRepository
    {
        Task<(IEnumerable<Student>, ServiceError)> GetAll();
        Task<(Student, ServiceError)> GetById(long id);
        Task<(long, ServiceError)> CreateStudent(string firstName, string lastName, decimal gpa = -1M);
        Task<(int, ServiceError)> UpdateStudent(long id, string firstName = null, string lastName = null, decimal? gpa = null);
        Task<(int, ServiceError)> Delete(long id);
    }
    
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentManagmentContext _context;

        public StudentRepository(StudentManagmentContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Student>, ServiceError)> GetAll()
        {
            try
            {
                var all = await _context.Students
                    .FromSql("EXECUTE dbo.SelectAllStudents")
                    .ToListAsync();
                return (all, null);
            }
            catch (Exception ex)
            {
                return (null, new ServiceError {Exception = ex, Message = ex.Message});
            }
        }
        public async Task<(Student, ServiceError)> GetById(long id)
        {
            try
            {
                var item = await _context.Students.FirstOrDefaultAsync(
                    s => s.Id == id);

                if(item == null) 
                {
                    return (null, null);
                }
                return (item, null);
            }
            catch (Exception ex)
            {
                return (null, new ServiceError {Exception = ex, Message = ex.Message});
            }
        }
        public async Task<(long, ServiceError)> CreateStudent(string firstName, string lastName, decimal gpa = -1M)
        {
            try
            {
                var id = await _context.Database
                    .ExecuteSqlCommandAsync($"EXECUTE dbo.InsertStudent {firstName},{lastName},{gpa}")
                    .ConfigureAwait(false);
                return (id, null);
            }
            catch (Exception ex)
            {
                return ((long)0, new ServiceError {Exception = ex, Message = ex.Message});
            }
        }
        public async Task<(int, ServiceError)> UpdateStudent(long id, 
            string firstName = null, string lastName = null, decimal? gpa = null)
        {
            try
            {
                if (gpa != null)
                {
                    gpa = Math.Round((decimal)gpa, 3);
                }
                var idParam = new SqlParameter("Id", id);
                var firstNameParam = new SqlParameter("FirstName", firstName);
                var lastNameParam = new SqlParameter("LastName", lastName);
                var gpaParam = new SqlParameter("Gpa", gpa);

                var r = await _context.Database
                    .ExecuteSqlCommandAsync("EXECUTE dbo.UpdateStudent @Id,@FirstName,@LastName,@Gpa", 
                        idParam, firstNameParam, lastNameParam, gpaParam)
                    .ConfigureAwait(false);

                return (r, null);
            }
            catch (Exception ex)
            {
                return (0, new ServiceError {Exception = ex, Message = ex.Message});
            }
        }
        public async Task<(int, ServiceError)>  Delete(long id)
        {
            try
            {
                var idParam = new SqlParameter("Id", id);
                var r = await _context.Database
                    .ExecuteSqlCommandAsync("EXECUTE dbo.DeleteStudentRecord @Id", idParam)
                    .ConfigureAwait(false);

                return (r, null);
            }
            catch (Exception ex)
            {
                return (0, new ServiceError {Exception = ex, Message = ex.Message});
            }
        }     
    }
}