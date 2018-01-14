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
        [HttpGet("{id}")]
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
        public async Task<IActionResult> Create(
            [FromBody]string first, 
            [FromBody] string last, 
            [FromBody] decimal gpa = 0M)
        {
            if (string.IsNullOrWhiteSpace(first) 
                || string.IsNullOrWhiteSpace(last))
            {
                return BadRequest();
            }
            var student = new Student 
            {
                FirstName = first, 
                LastName = last, 
                Gpa = gpa
            };
            
            _context.Students.Add(student);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return CreatedAtRoute("GetById", new { id = student.Id })
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
