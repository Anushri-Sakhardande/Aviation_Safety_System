using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Frontend
{
    public partial class Form7 : Form
    {
        OracleConnection conn;
        OracleCommand cmd;
        OracleDataAdapter da;
        DataTable dt;
        DataTable dtbase;
        public Form7(DataTable dt)
        {
            InitializeComponent();
            CollapseMenu();
            this.dt = dt;   
        }

        private void Form7_Load(object sender, EventArgs e)
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
            manucomboBox.DisplayMember = "manufacturer";

            //Type
            cmd.CommandText = "SELECT DISTINCT type FROM aircraft ORDER BY type ASC";
            cmd.CommandType = CommandType.Text;
            ds = new DataSet();
            da = new OracleDataAdapter(cmd.CommandText, conn);
            da.Fill(ds, "type");
            dtdp = ds.Tables["type"];
            typecomboBox.DataSource = dtdp.DefaultView;
            typecomboBox.DisplayMember = "type";

            //Country
            cmd.CommandText = "SELECT DISTINCT country FROM incident ORDER BY country ASC";
            cmd.CommandType = CommandType.Text;
            ds = new DataSet();
            da = new OracleDataAdapter(cmd.CommandText, conn);
            da.Fill(ds, "country");
            dtdp = ds.Tables["country"];
            countrycomboBox.DataSource = dtdp.DefaultView;
            countrycomboBox.DisplayMember = "country";
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
        }

        private void iconButtonCheck_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4(dt);
            this.Hide();
            form4.ShowDialog();
        }

        private void iconButtonStats_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(dt);
            this.Hide();
            form5.ShowDialog();
        }

        private void iconButtonReport_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6(dt);
            this.Hide();
            form6.ShowDialog();
        }

        private void iconButtonAccident_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7(dt);
            this.Hide();
            form7.ShowDialog();
        }

        private void iconButtonReview_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8(dt);
            this.Hide();
            form8.ShowDialog();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            conn.Open();
            string date = dateTimePicker.Text.Trim();
            string manufacturer = manucomboBox.Text.Trim();
            string aircraft = typecomboBox.Text.Trim();

            dtbase = new DataTable();
            string query = "select accd_date,location,fatalities,cat,country,type,manufacturer,mil_com from incident join aircraft using(aircraft_id)"; 

            OracleCommand cmd = new OracleCommand(query, conn);

            da = new OracleDataAdapter(cmd);
            da.Fill(dtbase);
            dataGridView1.DataSource = dtbase;
            conn.Close();

        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtbase);
            DateTime selectedDate = dateTimePicker.Value.Date; // Extracting date part without time
            dv.RowFilter = string.Format("accd_date = #{0:yyyy-MM-dd}#", selectedDate);
            dataGridView1.DataSource = dv;

        }

        private void manucomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtbase);
            string selectedManu = manucomboBox.Text; // Get the selected manufacturer
            dv.RowFilter = string.Format("manufacturer = '{0}'", selectedManu);
            dataGridView1.DataSource = dv;

        }

        private void typecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtbase);
            string selectedType = typecomboBox.Text; // Get the selected manufacturer
            dv.RowFilter = string.Format("type = '{0}'", selectedType);
            dataGridView1.DataSource = dv;
        }

        private void countrycomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dtbase);
            string selectedCountry = countrycomboBox.Text; // Get the selected manufacturer
            dv.RowFilter = string.Format("country = '{0}'", selectedCountry);
            dataGridView1.DataSource = dv;
        }
    }
}
