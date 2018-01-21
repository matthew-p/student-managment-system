using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Route("api/v1/[controller]")]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository Repo;

        public StudentsController(IStudentRepository repo) 
        {
            Repo = repo;
        }

        // GET api/v1/students
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            (var students, var err) = await Repo.GetAll().ConfigureAwait(false);
            if (err != null)
            {
                return StatusCode(500);
            }
            return Ok(students);
        }

        // GET api/v1/students/5
        [HttpGet("{id}", Name = "GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            (var student, var err) = await Repo.GetById(id).ConfigureAwait(false);

            if (err != null)
            {
                return StatusCode(500);
            }
            if(student == null) 
            {
                return NotFound();
            }
            return Ok(student);
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
            
            (var rows, var err) = await Repo.CreateStudent(firstName, lastName, gpa)
                .ConfigureAwait(false);

            if (err != null)
            {
                return StatusCode(500);
            }

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
            if ((string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
                || (gpa == null || gpa < 0 || gpa > 5M)) 
            {
                return BadRequest();
            }

            (var r, var err) = await Repo.UpdateStudent(id, firstName, lastName, gpa)
                .ConfigureAwait(false);
            
            if (err != null)
            {
                return StatusCode(500);
            }
            return new NoContentResult();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]long id)
        {
            var idParam = new SqlParameter("Id", id);
            (var r, var err) = await Repo.Delete(id).ConfigureAwait(false);

            if (err != null)
            {
                return StatusCode(500);
            }

            return new NoContentResult();
        }
    }
}
