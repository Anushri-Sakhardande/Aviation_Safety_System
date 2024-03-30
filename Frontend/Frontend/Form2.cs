using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Frontend
{
    public partial class Form2 : Form
    {
        OracleConnection conn;
        OracleCommand cmd;
        OracleDataAdapter da;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //TODO:string connectionString = ConfigurationManager.ConnectionStrings["Myconnectionstring"]?.ConnectionString;
            string connectionString = "Data Source=LAPTOP-B2QU8IP2;Persist Security Info=True;User ID=c##flysafe;Password=flysafeadminanujansin;";
            conn = new OracleConnection(connectionString);
            conn.Open();
        }

        private void loginbutton_Click(object sender, EventArgs e)
        {
            string username = SanitizeSql(nametextBox.Text);
            string email = SanitizeSql(emailtextBox.Text);
            string password = SanitizeSql(passtextBox.Text);
            string confirm = SanitizeSql(confirmtextBox.Text);   
            string phone = SanitizeSql(phonetextBox.Text);

            //check if user already exists
            string query = "select * from user_detail where email= '" + email + "'";
            da = new OracleDataAdapter(query, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                MessageBox.Show("User with this email id exists");
                clearText();
            }
            else
            {
                if(password==confirm && !IsValidEmail(email) && !IsValidPassword(password))
                {
                    MessageBox.Show("Incorrect Email or Password");
                }
                try
                {
                    byte[] salt;
                    new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                    var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

                    byte[] hash = pbkdf2.GetBytes(20);
                    byte[] hashBytes = new byte[36];

                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16,20);
                    string savedPasswordHash = Convert.ToBase64String(hashBytes);

                    cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO user_detail (username, password, email, phoneno) VALUES (:username, :password, :email, :phone)";

                    // Add parameters to the command
                    cmd.Parameters.Add(":username", OracleDbType.Varchar2).Value = username;
                    cmd.Parameters.Add(":password", OracleDbType.Varchar2).Value = password;
                    cmd.Parameters.Add(":email", OracleDbType.Varchar2).Value = email;
                    cmd.Parameters.Add(":phone", OracleDbType.Int64).Value = phone;

                    // Execute the query
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("User Created");

                    Form1 ob = new Form1();
                    this.Hide();
                    ob.ShowDialog();
                }
                catch
                {
                    MessageBox.Show("Error encountered:(\nTry Again");
                }
            }
        }

        private static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        private static bool IsValidPassword(string password)
        {
            // Password must contain at least one uppercase letter, one lowercase letter, one digit, and be at least 6 characters long
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$";
            return Regex.IsMatch(password, pattern);
        }

        public static string SanitizeSql(string input)
        {
            // Define a regular expression to match potentially dangerous SQL characters
            string pattern = @"[-;'\""]";

            // Replace potentially dangerous characters with empty strings
            string sanitizedInput = Regex.Replace(input, pattern, string.Empty);

            return sanitizedInput;
        }

        private void clearText()
        {
            this.nametextBox.Text="";
            this.emailtextBox.Text = "";
            this.passtextBox.Text = "";
            this.phonetextBox.Text = "";
        } 
    }
}
