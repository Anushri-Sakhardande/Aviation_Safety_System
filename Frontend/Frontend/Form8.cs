using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using FontAwesome.Sharp;


namespace Frontend
{
    public partial class Form8 : Form
    {
        OracleConnection conn;
        OracleCommand cmd;
        OracleDataAdapter da;
        DataTable dt;
        public Form8(DataTable dt)
        {
            InitializeComponent();
            CollapseMenu();
            this.dt = dt;
        }

        private void Form8_Load(object sender, EventArgs e)
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

            //Add all the reports to the table
            DataTable dtr = new DataTable();
            query = "select * from report";
            cmd=new OracleCommand(query, conn);
            da=new OracleDataAdapter(cmd);
            da.Fill(dt);
            int numberOfReports = dt.Rows.Count;

            tableLayoutPanel1.Controls.Clear();

            // Set up the TableLayoutPanel
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Loop to create labels and buttons
            for (int i = 0; i < numberOfReports; i++)
            {
                Label label = new Label();
                label.AutoSize = true;

                // Extract information from the current row of DataTable
                DataRow row = dt.Rows[i];
                string reportInfo = $"Report ID: {row["report_id"]}, Date: {row["report_datetime"]}, Location: {row["location"]}, Country: {row["country"]}";

                label.Text = reportInfo;
                // Create corresponding button

                iconButton1 = new IconButton
                {
                    // Set the icon using FontAwesome icon code
                    IconChar = IconChar.CheckCircle,
                    IconColor = System.Drawing.Color.Black,
                    IconFont = IconFont.Auto,
                    IconSize = 48,
                    Location = new System.Drawing.Point(50, 50), // Set the location
                    Size = new System.Drawing.Size(150, 50), // Set the size
                    TextAlign = ContentAlignment.MiddleRight, // Set the text alignment
                     // Set the text
                    FlatStyle = FlatStyle.Flat, // Set the button style
                    //BackColor = System.Drawing.Color., // Set the background color
                    ForeColor = System.Drawing.Color.White, // Set the text color
                    ImageAlign = ContentAlignment.MiddleLeft, // Set the image alignment
                    Image = null, // You can also set an image
                };
                // Attach event handler for button click event
                iconButton1.Click += (send, eve) =>
                {
                    // Handle button click event here
                    // You can use the sender object to identify which button was clicked
                    MessageBox.Show($"Button {((IconButton)sender).Name} clicked!");
                };

                // Add label and button to the TableLayoutPanel
                tableLayoutPanel1.Controls.Add(label, 0, i);
                tableLayoutPanel1.Controls.Add(iconButton1, 1, i);

            }
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
