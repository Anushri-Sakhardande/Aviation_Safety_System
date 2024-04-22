using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace Frontend
{
    public partial class Form5 : Form
    {
        OracleConnection conn;
        OracleCommand cmd;
        OracleDataAdapter da;
        DataTable dt;
        public Form5(DataTable dt)
        {
            InitializeComponent();
            CollapseMenu();
            this.dt = dt;
            FillChart();
        }

        private void Form5_Load(object sender, EventArgs e)
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
            conn.Close();
        }
        void FillChart()
        {
            string connectionString = Properties.Settings.Default.Connect;
            conn = new OracleConnection(connectionString);
            conn.Open();

            DataTable dt1 = new DataTable();
            OracleDataAdapter da1 = new OracleDataAdapter("select country, sum(fatalities) as fatalities from incident group by country", conn);
            da1.Fill(dt1);
            conn.Close();

            // Correct data binding
            chart1.DataSource = dt1;
            chart1.Series.Clear(); // Clear existing series
            chart1.Series.Add("Fatalities"); // Add a series
            chart1.Series["Fatalities"].XValueMember = "country";
            chart1.Series["Fatalities"].YValueMembers = "fatalities";
            chart1.Series["Fatalities"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

            chart1.Titles.Add("Fatalities by Country");

            // Axis configurations
            chart1.ChartAreas[0].AxisX.Title = "Country";
            chart1.ChartAreas[0].AxisY.Title = "Fatalities";

            chart1.ChartAreas[0].AxisX.Interval = 1; // Display every country name
            chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -45; // Rotate X-axis labels for better readability

            // Enable X-axis scrollbar
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;

            // Set the size of the scrollbars
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 10;

            // Set the minimum and maximum values for the X-axis
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = dt1.Rows.Count;

            // Set the maximum value for Y-axis
            chart1.ChartAreas[0].AxisY.Maximum = 15000; // Automatically set based on data

            // Refresh the chart
            chart1.DataBind();


            // chart1.AxisViewChanged += Chart1_AxisViewChanged;
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
