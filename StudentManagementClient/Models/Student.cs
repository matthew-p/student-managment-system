using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementClient.Models
{
    class Student
    {
            public long Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public decimal Gpa { get; set; }
    }
}
