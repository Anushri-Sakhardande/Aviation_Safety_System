using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Frontend
{
    public partial class Form3 : Form
    {
        OracleConnection conn;
        OracleCommand cmd;
        OracleDataAdapter da;
        DataTable dt;

        
        public Form3(DataTable dt)
        {
            InitializeComponent();
            CollapseMenu();
            this.dt = dt;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            string connectionString = Properties.Settings.Default.Connect;
            conn = new OracleConnection(connectionString);
            conn.Open();

            // Display the name
            nameLabel.Text = "Hello " + dt.Rows[0]["name"];

            string query = "SELECT MAX(login) AS recent_login FROM log_record WHERE user_id = :userId";
            cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(":userId", OracleDbType.Int32).Value = dt.Rows[0]["user_id"];
            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                DateTime mostRecentLogin = Convert.ToDateTime(result);
                loginsLabel.Text += " " + mostRecentLogin.ToString(); 
            }
            else
            {
                loginsLabel.Text = "Welcome new user";
            }

            //update the log record
            query = "INSERT INTO log_record (user_id, login) VALUES (:userId, SYSTIMESTAMP)";
            cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(":userId", OracleDbType.Int32).Value = dt.Rows[0]["user_id"];
            cmd.ExecuteNonQuery();

            //display number of reports 
            query = "SELECT COUNT(report_id) FROM report WHERE user_id = :userId";
            cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(":userId", OracleDbType.Int32).Value = dt.Rows[0]["user_id"];
            result = cmd.ExecuteScalar();
            reportedLabel.Text+= ":" + result.ToString();

            //display number of reports accepted
            query = "SELECT COUNT(report_id) FROM report WHERE user_id = :userId and accepted=1";
            cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(":userId", OracleDbType.Int32).Value = dt.Rows[0]["user_id"];
            result = cmd.ExecuteScalar();
            acceptedLabel.Text += ":" + result.ToString();

            //Points
            pointLabel.Text += ":" + dt.Rows[0]["points"];

            //check if admin
            int userId = Convert.ToInt32(dt.Rows[0]["user_id"]);
            query = "SELECT * FROM admin WHERE user_id = :userId";
            cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(":userId", OracleDbType.Int32).Value = userId;
            da = new OracleDataAdapter(cmd);

            DataTable dta = new DataTable();
            da.Fill(dta);
            if (dta.Rows.Count == 1)
            {
                iconButtonReview.Visible = true;
            }
        }


        private void label2_Click(object sender, EventArgs e)
        {

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
    }
}
