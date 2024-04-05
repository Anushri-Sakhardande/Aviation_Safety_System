using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Xml.Linq;

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
            string connectionString = Properties.Settings.Default.Connect;
            conn = new OracleConnection(connectionString);
            conn.Open();
        }

        private void loginbutton_Click(object sender, EventArgs e)
        {
            string name = SanitizeSql(nametextBox.Text);
            string email = SanitizeSql(emailtextBox.Text);
            string password = SanitizeSql(passtextBox.Text);
            string confirm = SanitizeSql(confirmtextBox.Text);   
            string phone = SanitizeSql(phonetextBox.Text);
            int points = 0;

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
                    //get userid
                    query = "SELECT count(*) FROM user_detail";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    int userId = Convert.ToInt32(cmd.ExecuteScalar())+1;


                    // Encrypt the password 
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
                    cmd.CommandText = "INSERT INTO user_detail (user_id, passhash, name, phone, points, email) VALUES (:userid, :passhash, :username, :phone, :points, :email)";

                    // Add parameters to the command
                    cmd.Parameters.Add(":userid", OracleDbType.Int32).Value = userId;
                    cmd.Parameters.Add(":passhash", OracleDbType.Varchar2).Value = savedPasswordHash;
                    cmd.Parameters.Add(":username", OracleDbType.Varchar2).Value = name;
                    cmd.Parameters.Add(":phone", OracleDbType.Int32).Value = phone;
                    cmd.Parameters.Add(":points", OracleDbType.Int32).Value = points;
                    cmd.Parameters.Add(":email", OracleDbType.Varchar2).Value = email;
                    //cmd.BindByName = true;

                    // Execute the query
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("User Created");

                    Form1 ob = new Form1();
                    this.Hide();
                    ob.ShowDialog();
                    Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error encountered:(\nTry Again");
                    MessageBox.Show(ex.Message);
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
            // Clear textboxes in case of error
            this.nametextBox.Text="";
            this.emailtextBox.Text = "";
            this.passtextBox.Text = "";
            this.phonetextBox.Text = "";
        } 
    }
}
