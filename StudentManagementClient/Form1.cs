using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StudentManagementClient.Models;
using Newtonsoft.Json;


namespace StudentManagementClient
{
    public partial class Form1 : Form
    {
        private static HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri("http://192.168.56.10:5000/api/v1/")
        };

        private Button GetAllStudentsButton;
        private Button CreateStudentButton;
        private Button DeleteStudentButton;
        //private StudentBinder Binder;
        private DataGridView MainGrid;

        public Form1()
        {
            //InitializeComponent();

            this.Size = new Size(500,500);

            GetAllStudentsButton = new Button();
            GetAllStudentsButton.Size = new Size(40, 40);
            GetAllStudentsButton.Location = new Point(30,30);
            GetAllStudentsButton.Text = "Get All Students";
            this.Controls.Add(GetAllStudentsButton);
            GetAllStudentsButton.Click += new EventHandler(GetAllStudentsButton_Click);

            CreateStudentButton = new Button();
            CreateStudentButton.Size = new Size(40, 40);
            CreateStudentButton.Location = new Point(70, 30);
            CreateStudentButton.Text = "Create Student";
            this.Controls.Add(CreateStudentButton);
            CreateStudentButton.Click += new EventHandler(CreateStudentButton_Click);

            DeleteStudentButton = new Button();
            DeleteStudentButton.Size = new Size(40, 40);
            DeleteStudentButton.Location = new Point(170, 30);
            DeleteStudentButton.Text = "Delete Student";
            this.Controls.Add(DeleteStudentButton);
            DeleteStudentButton.Click += new EventHandler(DeleteStudentButton_Click);

            //Binder = new StudentBinder(Client);
            MainGrid = new DataGridView();
            MainGrid.MultiSelect = false;
            MainGrid.DataSource = GetStudents();
            this.Controls.Add(MainGrid);
            MainGrid.Location = new Point(30, 80);
        }

        private async void GetAllStudentsButton_Click(object sender, EventArgs e)
        {
            var students = await GetStudents().ConfigureAwait(false);
            MainGrid.Invoke((Action)(() => MainGrid.DataSource = students));
            MainGrid.DataSource = students;
        }

        private async void CreateStudentButton_Click(object sender, EventArgs e)
        {
            var result = await CreateStudent(
                new Student {FirstName = "John", LastName = "Doe", Gpa = 1.0M});
            MessageBox.Show("Created Student ID: " + result.Id);
        }

        private async void DeleteStudentButton_Click(object sender, EventArgs e)
        {
            var selected = MainGrid.SelectedCells;
            if (selected.Count <= 0)
            {
                MessageBox.Show("No student selected");
                return; 
            }
            var student = selected[0].OwningRow
                .DataBoundItem as Student;
            if (student == null)
            {
                return;
            }

            DialogResult result = MessageBox.Show($"Do You Want to delete {student.LastName}, {student.FirstName}?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
            {
                return;
            }

            await DeleteStudent(student);
            return;
        }

        private static async Task<List<Student>> GetStudents()
        {
            List<Student> result = new List<Student>();
            try
            {
                using (HttpResponseMessage response = await Client
                    .GetAsync("students")
                    .ConfigureAwait(false))
                {
                    using (HttpContent content = response.Content)
                    {
                        string resultString = await content.ReadAsStringAsync();
                        HttpStatusCode statusCode = response.StatusCode;
                        if (statusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show($"Failed to retrieve result from server. Status Code: \n {statusCode}");
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

        private static async Task<Student> CreateStudent(Student student)
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
                        if (statusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show($"Failed to create the student. Status Code: \n {statusCode}");
                            return result;
                        }

                        result = JsonConvert.DeserializeObject<Student>(resultString);
                        MessageBox.Show("Created Student: " + statusCode);
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

        private static async Task UpdateStudent(Student student)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("firstName", student.FirstName),
                new KeyValuePair<string, string>("lastName", student.LastName),
                new KeyValuePair<string, string>("gpa", student.Gpa.ToString(CultureInfo.InvariantCulture))
            });
            try
            {
                using (HttpResponseMessage response = await Client
                    .PutAsync($"students/{student.Id}", formContent)
                    .ConfigureAwait(false))
                {
                    using (HttpContent content = response.Content)
                    {
                        string resultString = await content.ReadAsStringAsync();
                        HttpStatusCode statusCode = response.StatusCode;
                        if (statusCode != HttpStatusCode.NoContent)
                        {
                            MessageBox.Show($"Failed to update the student record. Status Code: \n {statusCode}");
                            return;
                        }
                        MessageBox.Show("Updated");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: \n" + ex.Message);
                return;
            }
        }

        private static async Task DeleteStudent(Student student)
        {
            try
            {
                var id = student.Id.ToString();
                using (HttpResponseMessage response = await Client
                    .DeleteAsync($"students/{id}")
                    .ConfigureAwait(false))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    if (statusCode != HttpStatusCode.NoContent)
                    {
                        MessageBox.Show($"Failed to delete the student. Status Code: \n {statusCode}");
                        return;
                    }
                    MessageBox.Show("Deleted");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: \n" + ex.Message);
                return;
            }
        }
    }
}
