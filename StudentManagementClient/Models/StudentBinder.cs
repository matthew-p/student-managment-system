using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace StudentManagementClient.Models
{
    public class StudentBinder : IList<Student>
    {
        internal List<Student> Students { get;  private set; }

        private HttpClient Client;
        private int _Count = 0;

        public StudentBinder(HttpClient client)
        {
            Client = client;
            Students = GetStudents()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public IEnumerator<Student> GetEnumerator()
        {
            Students = GetStudents().ConfigureAwait(false).GetAwaiter().GetResult();
            return Students.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Student item)
        {
            CreateStudent(item)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(Student item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(Student[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(Student item)
        {
            throw new System.NotImplementedException();
        }

        public int Count
        {
            get { return _Count; }
            set
            {
                Students = GetStudents()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
                _Count = Students.Count;
            }
        }
        public bool IsReadOnly { get; }
        public int IndexOf(Student item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(int index, Student item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public Student this[int index]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        private async Task<List<Student>> GetStudents()
        {
            List<Student> result = default(List<Student>);
            try
            {
                using (HttpResponseMessage response = await Client
                    .GetAsync("")
                    .ConfigureAwait(false))
                {
                    using (HttpContent content = response.Content)
                    {
                        string resultString = await content.ReadAsStringAsync();
                        HttpStatusCode statusCode = response.StatusCode;
                        if (statusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show($"Failed to retrieve result from server: \n {statusCode}");
                            return result;
                        }

                        result = JsonConvert.DeserializeObject<List<Student>>(resultString);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: \n" + ex.Message);
                return result;
            }
        }

        private async Task<Student> CreateStudent(Student student)
        {
            Student result = new Student();
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("firstName", student.FirstName),
                new KeyValuePair<string, string>("lastName", student.LastName),
                new KeyValuePair<string, string>("gpa", student.Gpa.ToString(CultureInfo.InvariantCulture))
            });
            try
            {
                using (HttpResponseMessage response = await Client
                    .PostAsync("students", formContent)
                    .ConfigureAwait(false))
                {
                    using (HttpContent content = response.Content)
                    {
                        string resultString = await content.ReadAsStringAsync();
                        HttpStatusCode statusCode = response.StatusCode;
                        if (statusCode != HttpStatusCode.Created)
                        {
                            MessageBox.Show($"Failed to retrieve result from server: \n {statusCode}");
                            return result;
                        }

                        result = JsonConvert.DeserializeObject<Student>(resultString);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: \n" + ex.Message);
                return result;
            }
        }
    }
}