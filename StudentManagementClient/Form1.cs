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

        private DataGridView MainGrid;

        private Button GetAllStudentsButton;
        private Button CreateStudentButton;
        private Button DeleteStudentButton;
        private Button EditStudentButton;

        private TextBox FirstNameText;
        private TextBox LastNameText;
        private TextBox GpaText;

        private Label FirstLabel;
        private Label LastLabel;
        private Label GpaLabel;

        private int ButtonWidth = 50;
        private int ButtonHeight = 20;
        private int TextBoxWidth = 150;
        private int TextBoxHeight = 20;
        

        public Form1()
        {
            Size = new Size(700,550);

            GetAllStudentsButton = new Button();
            GetAllStudentsButton.Size = new Size(ButtonWidth, ButtonHeight);
            GetAllStudentsButton.Location = new Point(30,30);
            GetAllStudentsButton.Text = "Get All";
            Controls.Add(GetAllStudentsButton);
            GetAllStudentsButton.Click += GetAllStudentsButton_Click;

            CreateStudentButton = new Button();
            CreateStudentButton.Size = new Size(ButtonWidth, ButtonHeight);
            CreateStudentButton.Location = new Point(80, 30);
            CreateStudentButton.Text = "Create";
            Controls.Add(CreateStudentButton);
            CreateStudentButton.Click += CreateStudentButton_Click;

            DeleteStudentButton = new Button();
            DeleteStudentButton.Size = new Size(ButtonWidth, ButtonHeight);
            DeleteStudentButton.Location = new Point(30, 50);
            DeleteStudentButton.Text = "Delete";
            Controls.Add(DeleteStudentButton);
            DeleteStudentButton.Click += DeleteStudentButton_Click;

            EditStudentButton = new Button();
            EditStudentButton.Size = new Size(ButtonWidth, ButtonHeight);
            EditStudentButton.Location = new Point(80, 50);
            EditStudentButton.Text = "Edit";
            Controls.Add(EditStudentButton);
            EditStudentButton.Click += EditStudentButton_Click;

            FirstNameText = new TextBox();
            FirstNameText.Multiline = false;
            FirstNameText.AcceptsReturn = false;
            FirstNameText.AcceptsTab = false;
            FirstNameText.MaxLength = 99;
            FirstNameText.Location = new Point(220, 30);
            FirstNameText.Size = new Size(TextBoxWidth, TextBoxHeight);
            Controls.Add(FirstNameText);

            LastNameText = new TextBox();
            LastNameText.Multiline = false;
            LastNameText.AcceptsReturn = false;
            LastNameText.AcceptsTab = false;
            LastNameText.MaxLength = 99;
            LastNameText.Location = new Point(220, 50);
            LastNameText.Size = new Size(TextBoxWidth, TextBoxHeight);
            Controls.Add(LastNameText);

            GpaText = new TextBox();
            GpaText.Multiline = false;
            GpaText.AcceptsReturn = false;
            GpaText.AcceptsTab = false;
            GpaText.MaxLength = 99;
            GpaText.Location = new Point(420,30);
            GpaText.Size = new Size(100, TextBoxHeight);
            Controls.Add(GpaText);

            FirstLabel = new Label();
            FirstLabel.Size = new Size(100, 20);
            FirstLabel.Text = "First Name:";
            FirstLabel.Location = new Point(150, 30);
            Controls.Add(FirstLabel);

            LastLabel = new Label();
            LastLabel.Size = new Size(100, 20);
            LastLabel.Text = "Last Name:";
            LastLabel.Location = new Point(150, 50);
            Controls.Add(LastLabel);

            GpaLabel = new Label();
            GpaLabel.Size = new Size(50, 20);
            GpaLabel.Text = "GPA:";
            GpaLabel.Location = new Point(380, 30);
            Controls.Add(GpaLabel);

            MainGrid = new DataGridView();
            MainGrid.ReadOnly = true;
            MainGrid.MultiSelect = false;
            MainGrid.DataSource = GetStudents();
            Controls.Add(MainGrid);
            MainGrid.Location = new Point(30, 80);
            MainGrid.Size = new Size(570, 400);
            MainGrid.SelectionChanged += MainGrid_SelectionChanged;
        }

        private async void MainGrid_SelectionChanged(object sender, EventArgs e)
        {
            var selected = MainGrid.SelectedCells;
            if (selected.Count <= 0)
            {
                return;
            }

            var student = selected[0].OwningRow
                .DataBoundItem as Student;
            FirstNameText.Text = student.FirstName;
            LastNameText.Text = student.LastName;
            GpaText.Text = student.Gpa.ToString();
        }

        

        private async void GetAllStudentsButton_Click(object sender, EventArgs e)
        {
            //var students = await GetStudents().ConfigureAwait(false);
            //MainGrid.Invoke((Action)(() => MainGrid.DataSource = students));
            //MainGrid.DataSource = students;
            await GetAll().ConfigureAwait(false);
        }

        private async Task GetAll()
        {
            var students = await GetStudents().ConfigureAwait(false);
            MainGrid.Invoke((Action)(() => MainGrid.DataSource = students));
            MainGrid.DataSource = students;
        }

        private async void CreateStudentButton_Click(object sender, EventArgs e)
        {
            var first = FirstNameText.Text;
            var last = LastNameText.Text;
            decimal gpa;
            var parsed = Decimal.TryParse(GpaText.Text, out gpa);
            if (!parsed)
            {
                MessageBox.Show("Could not parse GPA to decimal value");
                return;
            }
            await CreateStudent(
                new Student {FirstName = first, LastName = last, Gpa = gpa});
            MessageBox.Show("Created Student");
            await GetAll();
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
            await GetAll();
            return;
        }

        private async void EditStudentButton_Click(object sender, EventArgs e)
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

            var first = FirstNameText.Text;
            var last = LastNameText.Text;
            decimal gpa;
            var parsed = Decimal.TryParse(GpaText.Text, out gpa);
            if (!parsed)
            {
                MessageBox.Show("Could not parse GPA to decimal value");
                return;
            }

            DialogResult result = MessageBox.Show($"Do You Want to edit: {student.LastName}, {student.FirstName}?", "Edit", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
            {
                return;
            }
            student.FirstName = first;
            student.LastName = last;
            student.Gpa = gpa;

            await UpdateStudent(student);
            await GetAll();
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
