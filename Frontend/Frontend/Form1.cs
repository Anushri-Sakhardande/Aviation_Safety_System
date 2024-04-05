using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

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

            string query = "select * from user_detail where email= :email";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(":email", OracleDbType.Varchar2).Value = email;
            da = new OracleDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                //decrypt the password
                 string savedPasswordHash = dt.Rows[0][1].ToString();
                 byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);

                //compare results
                int ok = 1;
                for (int i = 0; i < 20; i++)
                    if (hashBytes[i + 16] != hash[i])
                        ok = 0;
                //if there are no differences between the strings, grant access
                if (ok == 1)
                {
                    
                    // Execute the query
                    cmd.ExecuteNonQuery();
                    Form3 ob = new Form3(dt);
                    this.Hide();
                    ob.ShowDialog();
                    Close();
                }
                else
                {
                    MessageBox.Show("Incorrect Password");
                }
            }
            else
            {
                MessageBox.Show("Incorrect Input:(\nIf you do not have an account register as a new user");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connectionString = Properties.Settings.Default.Connect;
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
