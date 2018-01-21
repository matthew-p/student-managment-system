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
        /// <summary>
        /// Fetches all non-deleted student records
        /// </summary>
        Task<(IEnumerable<Student>, ServiceError)> GetAll();

        /// <summary>
        /// Gets a single non-deleted student record by ID
        /// </summary>
        Task<(Student, ServiceError)> GetById(long id);

        /// <summary>
        /// Creates a new student record
        /// </summary>
        Task<(long, ServiceError)> CreateStudent(string firstName, string lastName, decimal gpa = -1M);

        /// <summary>
        /// Edits an existing student record
        /// </summary>
        Task<(int, ServiceError)> UpdateStudent(long id, string firstName = null, string lastName = null, decimal? gpa = null);

        /// <summary>
        /// Enables the deleted flag on a student record
        /// </summary>
        Task<(int, ServiceError)> Delete(long id);
    }
    
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentManagementContext _context;

        public StudentRepository(StudentManagementContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Student>, ServiceError)> GetAll()
        {
            try
            {
                var all = await _context.Students
                    .FromSql("EXECUTE dbo.SelectAllStudents")
                    .ToListAsync()
                    .ConfigureAwait(false);
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
                var item = await _context.Students
                    .FromSql($"EXECUTE dbo.SelectStudentById {id}")
                    .ToListAsync()
                    .ConfigureAwait(false);

                if(item?.FirstOrDefault() == null) 
                {
                    return (null, null);
                }
                return (item[0], null);
            }
            catch (Exception ex)
            {
                return (null, new ServiceError {Exception = ex, Message = ex.Message});
            }
        }

        public async Task<(long, ServiceError)> CreateStudent(string firstName = null, string lastName = null, decimal gpa = -1M)
        {
            try
            {
                gpa = Math.Round(gpa, 3);
                if (gpa < 0 || gpa > 5M)
                {
                    return (0, new ServiceError("GPA Out of Range"));
                }

                var firstNameParam = new SqlParameter("FirstName", firstName ?? (object)DBNull.Value);
                var lastNameParam = new SqlParameter("LastName", lastName ?? (object)DBNull.Value);
                var gpaParam = new SqlParameter("Gpa", gpa);

                var result = await _context.Database
                    .ExecuteSqlCommandAsync($"EXECUTE dbo.InsertStudent @FirstName,@LastName,@Gpa", 
                        firstNameParam, lastNameParam, gpaParam)
                    .ConfigureAwait(false);
                return (result, null);
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
                    if (gpa < 0 || gpa > 5M)
                    {
                        return (0, new ServiceError("GPA Out of Range"));
                    }
                }
                if (firstName == null)
                {
                    var x = DBNull.Value;
                }
                var idParam = new SqlParameter("Id", id);
                var firstNameParam = firstName != null 
                    ? new SqlParameter("FirstName", firstName) 
                    : new SqlParameter("FirstName", DBNull.Value);
                var lastNameParam = lastName != null 
                    ? new SqlParameter("LastName", lastName)
                    : new SqlParameter("LastName", DBNull.Value);
                var gpaParam = gpa != null 
                    ? new SqlParameter("Gpa", gpa)
                    : new SqlParameter("Gpa", DBNull.Value);

                var result = await _context.Database
                    .ExecuteSqlCommandAsync("EXECUTE dbo.UpdateStudent @Id,@FirstName,@LastName,@Gpa", 
                        idParam, firstNameParam, lastNameParam, gpaParam)
                    .ConfigureAwait(false);

                return (result, null);
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