using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Accord.Statistics.Models.Regression.Linear;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Accord.MachineLearning;



namespace Frontend
{
    public partial class Form4 : Form
    {
        OracleConnection conn;
        OracleCommand cmd;
        OracleDataAdapter da;
        DataTable dt;
        DataRow dr;
        int i = 0;
        MultipleLinearRegression regression;
        string abc;
        Dictionary<string, int> countryIds = new Dictionary<string, int>();
        Dictionary<string, int> manufacturerIds = new Dictionary<string, int>();

        public Form4(DataTable dt)
        {
            InitializeComponent();
            CollapseMenu();
            this.dt = dt;
        }

        private void Form4_Load(object sender, EventArgs e)
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

            // Populate the dropdowns
            DataSet ds = new DataSet();

            // Retrieve distinct manufacturers from aircraft table
            cmd.CommandText = "SELECT DISTINCT manufacturer FROM aircraft ORDER BY manufacturer ASC";
            cmd.CommandType = CommandType.Text;
            da = new OracleDataAdapter(cmd.CommandText, conn);
            da.Fill(ds, "manufacturers");

            // Set the DataSource for comboBox2
            comboBox2.DataSource = ds.Tables["manufacturers"].DefaultView;
            comboBox2.DisplayMember = "manufacturer";

            // Set the DataSource for comboBox3
            comboBox3.DataSource = ds.Tables["manufacturers"].DefaultView;
            comboBox3.DisplayMember = "manufacturer";

            // Set the DataSource for comboBox4
            comboBox4.DataSource = ds.Tables["manufacturers"].DefaultView;
            comboBox4.DisplayMember = "manufacturer";


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

        private void button1_Click(object sender, EventArgs e)
        {
            string fixedCountry = countrycomboBox.Text;
            string[] userManufacturers = { comboBox2.Text, comboBox3.Text, comboBox4.Text };
            cmd = new OracleCommand();
            cmd.CommandText = "SELECT m.MANUFACTURER AS manufacturer,c.COUNTRY AS country, SUM(c.FATALITIES) AS fatalities FROM AIRCRAFT m JOIN INCIDENT c ON m.AIRCRAFT_ID = c.AIRCRAFT_ID GROUP BY m.MANUFACTURER, c.COUNTRY";
            cmd.CommandType = CommandType.Text;
            DataSet ds = new DataSet();
            da = new OracleDataAdapter(cmd.CommandText, conn);
            da.Fill(ds, "AircraftIncidentData");

            dt = ds.Tables["AircraftIncidentData"];

            TrainLinearRegressionModel(dt);

            Dictionary<string, double> predictions = new Dictionary<string, double>();
            foreach (var manufacturer in userManufacturers)
            {
                double prediction = PredictFatalities(fixedCountry, manufacturer);
                predictions.Add(manufacturer, prediction);

            }
            StringBuilder predictionMessage = new StringBuilder();
            foreach (var kvp in predictions)
            {
                predictionMessage.AppendLine($"Manufacturer: {kvp.Key}, Predicted Fatalities: {kvp.Value}");
            }
            MessageBox.Show(predictionMessage.ToString());

            var bestManufacturer = predictions.OrderBy(kv => kv.Value).First();
            label6.Text = bestManufacturer.Key;

            conn.Close();

        }

        private void TrainLinearRegressionModel(DataTable dataTable)
        {
            int countryIdCounter = 1;
            int manufacturerIdCounter = 1;

            // Populate countryIds dictionary
            foreach (DataRow row in dataTable.Rows)
            {
                string country = row.Field<string>("country");

                if (!countryIds.ContainsKey(country))
                {
                    countryIds.Add(country, countryIdCounter);
                    countryIdCounter++;
                }
            }

            // Debug: Display countryIds dictionary contents
            foreach (var kvp in countryIds)
            {
                //MessageBox.Show($"Country: {kvp.Key}, ID: {kvp.Value}");
            }

            // Populate manufacturerIds dictionary
            foreach (DataRow row in dataTable.Rows)
            {
                string manufacturer = row.Field<string>("manufacturer");

                if (!manufacturerIds.ContainsKey(manufacturer))
                {
                    //MessageBox.Show($"Manufacturer '{manufacturer}'   not found in dictionary.");
                    manufacturerIds.Add(manufacturer, manufacturerIdCounter);
                    abc = manufacturer;
                    //MessageBox.Show($"manufacturer: {abc} and lengthg: {abc.Length} ");
                    StringBuilder message = new StringBuilder();

                    foreach (char c in abc)
                    {
                        //message.AppendLine($"Character: {c}, ASCII Value: {(int)c}");
                    }

                    // MessageBox.Show(message.ToString());
                    manufacturerIdCounter++;
                    //break;
                }
            }

            // Debug: Display manufacturerIds dictionary contents


            // Extract features and labels for training the regression model
            var features = dataTable.AsEnumerable()
                                     .Select(row => new double[] { countryIds[row.Field<string>("country")],
                                                   manufacturerIds[row.Field<string>("manufacturer")] })
                                     .ToArray();
            var labels = dataTable.AsEnumerable()
                                  .Select(row => (int)row.Field<decimal>("fatalities"))
                                  .Select(value => (double)value)
                                  .ToArray();
            // Train the linear regression model
            var teacher = new Accord.Statistics.Models.Regression.Linear.OrdinaryLeastSquares();
            regression = teacher.Learn(features, labels);
        }




        private double PredictFatalities(string country, string manufacturer1)
        {
            // Get the unique numeric identifier for the country

            MessageBox.Show($"Predicting fatalities for: Country: {country}, Manufacturer: {manufacturer1}");

            // Trim whitespace from the manufacturer name
            manufacturer1 = manufacturer1.Trim();


            int countryId = countryIds.ContainsKey(country) ? countryIds[country] : 0;
            MessageBox.Show($"Country: {country}, Country ID: {countryId}");

            foreach (var kvp in manufacturerIds)
            {
                // MessageBox.Show($"Manufacturer: {kvp.Key}, ID: {kvp.Value}");
            }

            // Get the unique numeric identifier for the manufacturer
            int manufacturerId = manufacturerIds.ContainsKey(manufacturer1 + " ") ? manufacturerIds[manufacturer1 + " "] : 0;
            //int manufacturerId = manufacturerIds[];
            MessageBox.Show($"Manufacturer: {manufacturer1}, Manufacturer ID: {manufacturerId}");

            // int manufacturerId = 0;



            // Predict the number of fatalities
            double[] input = { countryId, manufacturerId };
            double prediction = regression.Transform(input);
            MessageBox.Show($"Prediction: {prediction}");

            return prediction;

        }
    }

}