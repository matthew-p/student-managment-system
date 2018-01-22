using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.Repositories;
using Microsoft.Extensions.Logging;

namespace StudentManagement.Controllers
{
    [Route("api/v1/[controller]")]
    public class StudentsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IStudentRepository _repo;

        public StudentsController(ILogger<StudentsController> logger, IStudentRepository repo) 
        {
            _logger = logger;
            _repo = repo;
        }

        // GET api/v1/students
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            (var students, var err) = await _repo.GetAll().ConfigureAwait(false);
            if (err != null)
            {
                _logger.LogError($"Failed to get all students: {err.Message}");
                return StatusCode(500);
            }
            return Ok(students);
        }

        // GET api/v1/students/5
        [HttpGet("{id}", Name = "GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            (var student, var err) = await _repo.GetById(id).ConfigureAwait(false);

            if (err != null)
            {
                _logger.LogError($"Failed to get student {id}: {err.Message}");
                return StatusCode(500);
            }
            if(student == null) 
            {
                _logger.LogDebug($"Requested student ID: {id}, was not found");
                return NotFound();
            }
            return Ok(student);
        }

        // POST api/v1/students
        [HttpPost]
        public async Task<IActionResult> CreateStudent(
            string firstName, 
            string lastName, 
            decimal gpa = 0M)
        {
            if (string.IsNullOrWhiteSpace(firstName) 
                || string.IsNullOrWhiteSpace(lastName)
                || gpa < 0 || gpa > 5M)
            {
                _logger.LogDebug("Improper values provided to create student");
                return BadRequest();
            }

            var student = new Student 
            {
                FirstName = firstName, 
                LastName = lastName, 
                Gpa = gpa
            };
            
            (var result, var err) = await _repo.CreateStudent(firstName, lastName, gpa)
                .ConfigureAwait(false);

            if (err != null)
            {
                _logger.LogError($"Failed to create student: {err.Message}");
                return StatusCode(500);
            }
            _logger.LogDebug($"Created student: {result}");
            return StatusCode(201);
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
                _logger.LogDebug("Improper values given to update student");
                return BadRequest();
            }

            (var r, var err) = await _repo.UpdateStudent(id, firstName, lastName, gpa)
                .ConfigureAwait(false);
            
            if (err != null)
            {
                _logger.LogError($"Failed to update student ID: {id}: {err.Message}");
                return StatusCode(500);
            }
            _logger.LogDebug($"Updated Student ID: {id}: {r}");
            return new NoContentResult();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]long id)
        {
            (var r, var err) = await _repo.Delete(id).ConfigureAwait(false);

            if (err != null)
            {
                _logger.LogError($"Failed to delete student ID {id}: {err.Message}");
                return StatusCode(500);
            }
            _logger.LogDebug($"Deleted student ID {id}: {r}");
            return new NoContentResult();
        }
    }
}
