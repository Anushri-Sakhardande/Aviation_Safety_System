using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Frontend
{
    public partial class Form1 : Form
    {
        OracleConnection conn;
        OracleDataAdapter da;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            this.Hide();
            form2.ShowDialog();
        }

        private void loginbutton_Click(object sender, EventArgs e)
        {
            string email = SanitizeSql(emailtextBox.Text);
            string password = SanitizeSql(passtextBox.Text); 

            string query = "select * from user_detail where email= '"+email+"' and password= '"+password+"'";
            da = new OracleDataAdapter(query,conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                Form3 form3 = new Form3(dt);
                this.Hide();
                form3.ShowDialog();
            }
            else
            {
                MessageBox.Show("Incorrect Input:(\nIf you do not have an account register as a new user");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //TODO:string connectionString = ConfigurationManager.ConnectionStrings["Myconnectionstring"]?.ConnectionString;
            string connectionString = "Data Source=LAPTOP-B2QU8IP2;Persist Security Info=True;User ID=c##flysafe;Password=flysafeadminanujansin;";
            conn = new OracleConnection(connectionString);
            conn.Open();
        }

        public static string SanitizeSql(string input)
        {
            // Define a regular expression to match potentially dangerous SQL characters
            string pattern = @"[-;'\""]";

            // Replace potentially dangerous characters with empty strings
            string sanitizedInput = Regex.Replace(input, pattern, string.Empty);

            return sanitizedInput;
        }

    }
}
