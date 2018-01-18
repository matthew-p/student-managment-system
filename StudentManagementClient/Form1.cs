using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public Button button1;

        public Form1()
        {
            button1 = new Button();
            button1.Size = new Size(40, 40);
            button1.Location = new Point(30,30);
            button1.Text = "Click me";
            this.Controls.Add(button1);
            button1.Click += new EventHandler(button1_Click);
           // InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello World");
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
                            MessageBox.Show($"Failed to fetch student list: \n {statusCode.GetTypeCode()}");
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
    }
}
