using System;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentManagement.Models;
using StudentManagement.Repositories;
using System.Threading;

namespace StudentManagementTests
{
    public class StudentManagementManualTests : IDisposable
    {
        private IConfigurationRoot Configuration { get; set; }

        private StudentManagmentContext Cntx { get; set; }

        private StudentRepository Repo { get; set; }

        public StudentManagementManualTests()
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("./appsettings.json",
                    optional: false, reloadOnChange: true);
            Configuration = configBuilder.Build();

            var conn = Configuration.GetConnectionString("SchoolDatabase");

            var builder = new DbContextOptionsBuilder<StudentManagmentContext>();
            builder.UseSqlServer(conn);
            var opts = builder.Options;
            Cntx = new StudentManagmentContext(opts);

            Repo = new StudentRepository(Cntx);
        }

        private void RemoveTestStudents()
        {
            Cntx.Database
                .ExecuteSqlCommandAsync("DELETE FROM Students WHERE LastName = 'TestClass'")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private void AddTestStudent()
        {
            Cntx.Database
                .ExecuteSqlCommandAsync("INSERT INTO Students(FirstName,LastName,Gpa) VALUES('John', 'TestClass', 2.5)")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private Student GetTestStudent(long id)
        {
            return Cntx.Students
                .FromSql($"SELECT * FROM Students WHERE Id = {id}")
                .ToList()
                .First();
        }

        [Fact]
        public void GetAllStudents_Works_GivenStudentsInDb()
        {
            AddTestStudent();
            AddTestStudent();
            (var students, var err) = Repo.GetAll().Result;

            Assert.Null(err);
            Assert.NotEmpty(students);
            Assert.NotEmpty(students.Where(s => s.LastName == "TestClass"));
        }

        [Fact]
        public void CreateStudent_Works()
        {
            (var id, var err) = Repo.CreateStudent("CreateTest", "TestClass", 3.1M)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.Null(err);

            (var students, var allErr) = Repo.GetAll()
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.Null(allErr);
            Assert.NotEmpty(students);
            Assert.NotEmpty(students.Where(s => s.FirstName == "CreateTest"));
        }

        [Fact]
        public void CreateStudent_Works_WithNull()
        {
            (var id, var err) = Repo.CreateStudent(null, "TestClass", 3.2M)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.Null(err);

            (var students, var allErr) = Repo.GetAll()
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.Null(allErr);
            Assert.NotEmpty(students);
            Assert.NotEmpty(students.Where(s => s.Gpa == 3.2M));
        }

        [Fact]
        public void UpdateStudent_Works()
        {          
            (var something, var err2) = Repo.UpdateStudent(162, "UpdatedName", "UpdatedName")
                .ConfigureAwait(false).GetAwaiter().GetResult();

            var result = GetTestStudent(162);
            Assert.Equal("UpdatedName", result.LastName);
        }

        [Fact]
        public void DeleteStudent_Works()
        {
            AddTestStudent();
            (var students, var err1) = Repo.GetAll().ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(err1);
            Assert.NotEmpty(students);

            var deleteable = students.First(s => s.LastName == "TestClass");
            (var result, var err2) = Repo.Delete(deleteable.Id)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            (var deleted, var err3) = Repo.GetById(deleteable.Id)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Null(deleted);
            Assert.Null(err3);
        }

        public void Dispose()
        {
            RemoveTestStudents();
            Cntx.Dispose();
        }
    }
}
