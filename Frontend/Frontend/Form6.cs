using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using System.Globalization;

namespace Frontend
{
    public partial class Form6 : Form
    {
        OracleConnection conn;
        OracleCommand cmd;
        OracleDataAdapter da;
        DataTable dt;
        public Form6(DataTable dt)
        {
            InitializeComponent();
            CollapseMenu();
            this.dt = dt;   
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            string connectionString = Properties.Settings.Default.Connect;
            conn = new OracleConnection(connectionString);
            conn.Open();

            //check if admin
            int userId = Convert.ToInt32(dt.Rows[0]["user_id"]);
            string query = "SELECT * FROM admin WHERE user_id = :userId";
            cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(":userId", OracleDbType.Int32).Value = userId;
            da = new OracleDataAdapter(cmd);

            DataTable dta = new DataTable();
            da.Fill(dta);
            if (dta.Rows.Count == 1)
            {
                iconButtonReview.Visible = true;
            }

            // Populate the dropdowns
            DataTable dtdp = new DataTable();   

            //manufacturer
            cmd.CommandText = "SELECT DISTINCT manufacturer FROM aircraft ORDER BY manufacturer ASC";
            cmd.CommandType = CommandType.Text;
            DataSet ds = new DataSet();
            da = new OracleDataAdapter(cmd.CommandText, conn);
            da.Fill(ds, "manufacturer");
            dtdp = ds.Tables["manufacturer"];
            manucomboBox.DataSource = dtdp.DefaultView;
            manucomboBox.DisplayMember = "Manufacturer";

            //cat
            cmd.CommandText = "SELECT DISTINCT cat FROM incident ORDER BY cat ASC";
            cmd.CommandType = CommandType.Text;
            ds = new DataSet();
            da = new OracleDataAdapter(cmd.CommandText, conn);
            da.Fill(ds, "cat");
            dtdp = ds.Tables["cat"];
            catcomboBox.DataSource = dtdp.DefaultView;
            catcomboBox.DisplayMember = "Cat";

            //Type
            cmd.CommandText = "SELECT DISTINCT type FROM aircraft ORDER BY type ASC";
            cmd.CommandType = CommandType.Text;
            ds = new DataSet();
            da = new OracleDataAdapter(cmd.CommandText, conn);
            da.Fill(ds, "type");
            dtdp = ds.Tables["type"];
            typecomboBox.DataSource = dtdp.DefaultView;
            typecomboBox.DisplayMember = "Type";

            //Country
            cmd.CommandText = "SELECT DISTINCT country FROM incident ORDER BY country ASC";
            cmd.CommandType = CommandType.Text;
            ds = new DataSet();
            da = new OracleDataAdapter(cmd.CommandText, conn);
            da.Fill(ds, "country");
            dtdp = ds.Tables["country"];
            countrycomboBox.DataSource = dtdp.DefaultView;
            countrycomboBox.DisplayMember = "Country";
            conn.Close();

            //operator
            cmd.CommandText = "SELECT DISTINCT operator FROM registration ORDER BY operator ASC";
            cmd.CommandType = CommandType.Text;
            ds = new DataSet();
            da = new OracleDataAdapter(cmd.CommandText, conn);
            da.Fill(ds, "operator");
            dtdp = ds.Tables["operator"];
            operatorcomboBox.DataSource = dtdp.DefaultView;
            operatorcomboBox.DisplayMember = "Operator";
            conn.Close();

        }

        //!!! REPLICABLE CODE
        private void CollapseMenu()
        {
            if (this.panelMenu.Width > 200) //Collapse menu
            {
                panelMenu.Width = 100;
                label8.Visible = false;
                iconButton1.Dock = DockStyle.Top;
                foreach (Button menuButton in panelMenu.Controls.OfType<Button>())
                {
                    menuButton.Text = "";
                    menuButton.ImageAlign = ContentAlignment.MiddleCenter;
                    menuButton.Padding = new Padding(0);
                }
            }
            else
            { //Expand menu
                panelMenu.Width = 269;
                label8.Visible = true;
                iconButton1.Dock = DockStyle.None;
                foreach (Button menuButton in panelMenu.Controls.OfType<Button>())
                {
                    menuButton.Text = "   " + menuButton.Tag.ToString();
                    menuButton.ImageAlign = ContentAlignment.MiddleLeft;
                    menuButton.Padding = new Padding(10, 10, 10, 10);
                }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            CollapseMenu();
        }


        private void iconButtonHome_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(dt);
            this.Hide();
            form3.ShowDialog();
            Close();
        }

        private void iconButtonCheck_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4(dt);
            this.Hide();
            form4.ShowDialog();
            Close();
        }

        private void iconButtonStats_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(dt);
            this.Hide();
            form5.ShowDialog();
            Close();
        }

        private void iconButtonReport_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6(dt);
            this.Hide();
            form6.ShowDialog();
            Close();
        }

        private void iconButtonAccident_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7(dt);
            this.Hide();
            form7.ShowDialog();
            Close();
        }

        private void iconButtonReview_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8(dt);
            this.Hide();
            form8.ShowDialog();
            Close();
        }

        private void submitbutton_Click(object sender, EventArgs e)
        {
            DateTime dateTime = dateTimePicker.Value.Date;
            string fatalitiesStr = fatalitiesnumericUpDown.Text;
            string operate = operatorcomboBox.Text;
            string registration = registrationtextBox.Text;
            string location = locationtextBox.Text;
            string country = countrycomboBox.Text;
            string type = typecomboBox.Text;
            string manu = manucomboBox.Text;
            string cat = catcomboBox.Text;

            int fatalities;
            bool parsedSuccessfully = int.TryParse(fatalitiesStr, out fatalities);

            try
            {
                if (parsedSuccessfully && fatalities >= 0 &&
                    !string.IsNullOrEmpty(operate) &&
                    !string.IsNullOrEmpty(registration) &&
                    !string.IsNullOrEmpty(location) &&
                    !string.IsNullOrEmpty(country) &&
                    !string.IsNullOrEmpty(type) &&
                    !string.IsNullOrEmpty(manu) &&
                    !string.IsNullOrEmpty(cat))
                {
                    conn.Open(); // Open the connection

                    // Get the user ID
                    string userId = dt.Rows[0]["user_id"].ToString();

                    //get Report id
                    string query = "SELECT count(*) FROM report";
                    cmd = new OracleCommand(query, conn);
                    int reportId = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

                    //Miltary or Commercial
                    char mil_Com = milradioButton.Checked ? 'M' : 'C';

                    // Prepare the insert query
                    string insertQuery = "INSERT INTO report (user_id, report_id, report_datetime, manufacturer, registration, type, mil_Com, accident_date, operator, fatalities, location, country, cat, accepted) " +
                                         "VALUES (:userId, :reportId, SYSTIMESTAMP, :manufacturer, :registration, :type, :mil_Com, :accident_date, :operate, :fatalities, :location, :country, :cat, :accepted)";
                    cmd = new OracleCommand(insertQuery, conn);

                    // Add parameters to the command
                    cmd.Parameters.Add(":userId", OracleDbType.Int32).Value = userId;
                    cmd.Parameters.Add(":reportId", OracleDbType.Int32).Value = reportId;
                    cmd.Parameters.Add(":manufacturer", OracleDbType.Varchar2).Value = manu;
                    cmd.Parameters.Add(":registration", OracleDbType.Varchar2).Value = registration;
                    cmd.Parameters.Add(":type", OracleDbType.Varchar2).Value = type;
                    cmd.Parameters.Add(":mil_Com", OracleDbType.Varchar2).Value = mil_Com;
                    cmd.Parameters.Add(":accident_date", OracleDbType.Date).Value = dateTime;
                    cmd.Parameters.Add(":operate", OracleDbType.Varchar2).Value = operate;
                    cmd.Parameters.Add(":fatalities", OracleDbType.Int32).Value = fatalities;
                    cmd.Parameters.Add(":location", OracleDbType.Varchar2).Value = location;
                    cmd.Parameters.Add(":country", OracleDbType.Varchar2).Value = country;
                    cmd.Parameters.Add(":cat", OracleDbType.Varchar2).Value = cat;
                    cmd.Parameters.Add(":accepted", OracleDbType.Int32).Value = 0;

                    // Execute the query
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Your report will be considered by the admin :)");
                }
                else
                {
                    MessageBox.Show("Incorrect Input :(");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.StackTrace);
                
            }
            finally
            {
                conn.Close();
            }

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
