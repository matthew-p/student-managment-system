using System;

namespace StudentManagement.Models
{
    public class ServiceError
    {
        public Exception Exception { get; set; } = null;
        public string Message { get; set; } = "";
    }
    
}