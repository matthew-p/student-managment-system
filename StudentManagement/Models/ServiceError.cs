using System;

namespace StudentManagement.Models
{
    public class ServiceError
    {
        public Exception Exception { get; set; } = null;
        public string Message { get; set; } = "";

        public ServiceError()
        {}
        public ServiceError(string message)
        {
            Message = Message;
        }
        public ServiceError(Exception ex)
        {
            Exception = ex;
        }
    }
    
}