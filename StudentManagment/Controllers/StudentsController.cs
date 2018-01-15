using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentManagment.Models;

namespace StudentManagment.Controllers
{
    [Route("api/v1/[controller]")]
    public class StudentsController : Controller
    {
        private readonly StudentManagmentContext _context;

        public StudentsController(StudentManagmentContext context)
        {
            _context = context;
        }

        // GET api/v1/students
        [HttpGet]
        public IEnumerable<Student> GetAll()
        {
            var all = _context.Students.ToList();
            return all;
        }

        // GET api/v1/students/5
        [HttpGet("{id}", Name = "GetById")]
        public IActionResult GetById(long id)
        {
            var item = _context.Students.FirstOrDefault(
                s => s.Id == id);

            if(item == null) 
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST api/v1/students
        [HttpPost]
        public async Task<IActionResult> CreateStudent(
            string firstName, 
            string lastName, 
            decimal gpa = -1M)
        {
            if (string.IsNullOrWhiteSpace(firstName) 
                || string.IsNullOrWhiteSpace(lastName))
            {
                return BadRequest();
            }
            var student = new Student 
            {
                FirstName = firstName, 
                LastName = lastName, 
                Gpa = gpa
            };
            
            _context.Students.Add(student);
            var rows = await _context.SaveChangesAsync().ConfigureAwait(false);
            var idObj = new { id = student.Id };
            return CreatedAtRoute("GetById", idObj, student);
        }

        // PUT api/v1/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(
            [FromRoute]long id, 
            string firstName = null, 
            string lastName = null, 
            decimal? gpa = null)
        {
            if (string.IsNullOrWhiteSpace(firstName) 
                && string.IsNullOrWhiteSpace(lastName)
                && gpa == null)
            {
                return BadRequest();
            }

            var student = _context.Students.FirstOrDefault(
                s => s.Id == id
            );
            if (student == null) 
            {
                return NotFound();
            }

            student.FirstName = firstName == null 
                ? student.FirstName 
                : firstName;
            student.LastName = lastName == null 
                ? student.LastName 
                : lastName;
            student.Gpa = gpa == null 
                ? student.Gpa
                : (decimal)gpa;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]long id)
        {
            var student = _context.Students.FirstOrDefault(
                s => s.Id == id
            );
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return new NoContentResult();
        }
    }
}
