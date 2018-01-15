using System;
using System.Collections.Generic;
using System.Linq;
using StudentManagment.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace StudentManagment.Repositories
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
                // TODO fix
                var r = _context.Database
                .ExecuteSqlCommand($"EXECUTE dbo.InsertStudent {firstName},{lastName},{gpa}");
                var rows = await _context.SaveChangesAsync().ConfigureAwait(false);
                return (r, null);
            }
            catch (Exception ex)
            {
                return ((long)0, new ServiceError {Exception = ex, Message = ex.Message});
            }
        }
        public async Task<(int, ServiceError)> UpdateStudent(long id, string firstName = null, string lastName = null, decimal? gpa = null)
        {
            try
            {
                var idParam = new SqlParameter("Id", id);
                var firstNameParam = new SqlParameter("FirstName", firstName);
                var lastNameParam = new SqlParameter("LastName", lastName);
                var gpaParam = new SqlParameter("Gpa", gpa);

                // TODO improve
                _context.Database
                .ExecuteSqlCommand("EXECUTE dbo.UpdateStudent @Id,@FirstName,@LastName,@Gpa", 
                    idParam, firstNameParam, lastNameParam, gpaParam);
                var r = await _context.SaveChangesAsync();
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
                _context.Database
                    .ExecuteSqlCommand("EXECUTE dbo.DeleteStudentRecord @Id", idParam);
                var r = await _context.SaveChangesAsync().ConfigureAwait(false);
                return (r, null);
            }
            catch (Exception ex)
            {
                return (0, new ServiceError {Exception = ex, Message = ex.Message});
            }
        }     
    }
}