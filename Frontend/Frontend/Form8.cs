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
using System.Collections;


namespace Frontend
{
    public partial class Form8 : Form
    {
        OracleConnection conn;
        OracleCommand cmd;
        OracleDataAdapter da;
        DataTable dt;
        private IconButton iconButtonAccept;
        private IconButton iconButtonCancel;

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

        }
        
        private void clearTable()
        {
        tableLayoutPanel1.Controls.Clear();
        tableLayoutPanel1.RowStyles.Clear();
        tableLayoutPanel1.ColumnStyles.Clear();
        tableLayoutPanel1.RowCount = 1; 
            for (int i = tableLayoutPanel1.Controls.Count - 1; i >= 0; i--)
            {
                if (tableLayoutPanel1.GetRow(tableLayoutPanel1.Controls[i]) > 0)
                {
                    Control control = tableLayoutPanel1.Controls[i];
                    tableLayoutPanel1.Controls.Remove(control);
                    control.Dispose();
                }
            }
        }
        
        private void showTable()
        {
            clearTable();
            //Add all the reports to the table
            DataTable dtr = new DataTable();
            string query = "select * from report where accepted=0";
            cmd = new OracleCommand(query, conn);
            da = new OracleDataAdapter(cmd);
            da.Fill(dt);
            int numberOfReports = dt.Rows.Count;

            // Loop to create labels and buttons
            for (int i = 0; i < numberOfReports; i++)
            {
                DataRow row = dt.Rows[i];
                //1
                Label userlabel = new Label();
                userlabel.AutoSize = true;
                userlabel.Text = row["user_id"].ToString();
                //2
                Label repDatelabel = new Label();
                repDatelabel.AutoSize = true;
                repDatelabel.Text = row["report_datetime"].ToString();
                //3
                Label manufacturerlabel = new Label();
                manufacturerlabel.AutoSize = true;
                manufacturerlabel.Text = row["manufacturer"].ToString();
                //4
                Label reglabel = new Label();
                reglabel.AutoSize = true;
                reglabel.Text = row["registration"].ToString();
                //5
                Label typelabel = new Label();
                typelabel.AutoSize = true;
                typelabel.Text = row["type"].ToString();
                //6
                Label milcommlabel = new Label();
                milcommlabel.AutoSize = true;
                milcommlabel.Text = row["mil_Com"].ToString();
                //7
                Label datelabel = new Label();
                datelabel.AutoSize = true;
                datelabel.Text = row["accident_date"].ToString();
                //8
                Label operatorlabel = new Label();
                operatorlabel.AutoSize = true;
                operatorlabel.Text = row["operator"].ToString();
                //9
                Label fatalitieslabel = new Label();
                fatalitieslabel.AutoSize = true;
                fatalitieslabel.Text = row["fatalities"].ToString();
                //11
                Label locationlabel = new Label();
                locationlabel.AutoSize = true;
                locationlabel.Text = row["location"].ToString();
                //12
                Label countrylabel = new Label();
                countrylabel.AutoSize = true;
                countrylabel.Text = row["country"].ToString();
                //13
                Label catlabel = new Label();
                catlabel.AutoSize = true;
                catlabel.Text = row["cat"].ToString();

                this.iconButtonAccept = new FontAwesome.Sharp.IconButton();
                this.iconButtonAccept.IconChar = FontAwesome.Sharp.IconChar.CheckCircle;
                this.iconButtonAccept.IconColor = System.Drawing.Color.Green;
                this.iconButtonAccept.IconFont = FontAwesome.Sharp.IconFont.Auto;
                this.iconButtonAccept.IconSize = 24;
                this.iconButtonAccept.Name = "iconButtonAccept";
                this.iconButtonAccept.Size = new System.Drawing.Size(40, 40); // Set size as needed
                this.iconButtonAccept.TabIndex = 0;
                this.iconButtonAccept.Text = "Accept";
                this.iconButtonAccept.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
                this.iconButtonAccept.UseVisualStyleBackColor = true;
                this.iconButtonAccept.Tag = row["report_id"].ToString();
                this.iconButtonAccept.Click += new System.EventHandler(this.iconButtonAccept_Click);

                // Initialize iconButtonCancel
                this.iconButtonCancel = new FontAwesome.Sharp.IconButton();
                this.iconButtonCancel.IconChar = FontAwesome.Sharp.IconChar.TimesCircle;
                this.iconButtonCancel.IconColor = System.Drawing.Color.Red;
                this.iconButtonCancel.IconFont = FontAwesome.Sharp.IconFont.Auto;
                this.iconButtonCancel.IconSize = 24;
                this.iconButtonCancel.Name = "iconButtonCancel";
                this.iconButtonCancel.Size = new System.Drawing.Size(40, 40); // Set size as needed
                this.iconButtonCancel.TabIndex = 1;
                this.iconButtonCancel.Text = "Reject";
                this.iconButtonCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
                this.iconButtonCancel.UseVisualStyleBackColor = true;
                this.iconButtonCancel.Tag = row["report_id"].ToString();
                this.iconButtonCancel.Click += new System.EventHandler(this.iconButtonCancel_Click);

                // Add controls to tableLayoutPanel1
                tableLayoutPanel1.Controls.Add(iconButtonAccept, 12, i+1);
                tableLayoutPanel1.Controls.Add(iconButtonCancel, 13, i+1);

                tableLayoutPanel1.Controls.Add(userlabel, 0, i + 1);
                tableLayoutPanel1.Controls.Add(repDatelabel, 1, i + 1);
                tableLayoutPanel1.Controls.Add(reglabel, 2, i + 1);
                tableLayoutPanel1.Controls.Add(locationlabel, 3, i + 1);
                tableLayoutPanel1.Controls.Add(countrylabel, 4, i + 1);
                tableLayoutPanel1.Controls.Add(manufacturerlabel, 5, i + 1);
                tableLayoutPanel1.Controls.Add(typelabel, 6, i + 1);
                tableLayoutPanel1.Controls.Add(milcommlabel, 7, i + 1);
                tableLayoutPanel1.Controls.Add(datelabel, 8, i + 1);
                tableLayoutPanel1.Controls.Add(operatorlabel, 9, i + 1);
                tableLayoutPanel1.Controls.Add(fatalitieslabel, 11, i + 1);
                tableLayoutPanel1.Controls.Add(catlabel, 10, i + 1);

            }
            tableLayoutPanel1.Visible = true;
        }

        private void iconButtonCancel_Click(object sender, EventArgs e)
        {
            int reportId = 0;
            if (int.TryParse(((IconButton)sender).Tag.ToString(), out reportId))
            {
                // Update ACCEPTED to 1
                UpdateAcceptedStatus(reportId, 3);
            }
            else
            {
                MessageBox.Show("Invalid report ID.");
            }
        }

        private void iconButtonAccept_Click(object sender, EventArgs e)
        {
            int reportId = 0;
            if (int.TryParse(((IconButton)sender).Tag.ToString(), out reportId))
            {
                // Update ACCEPTED to 3
                UpdateAcceptedStatus(reportId, 1);
            }
            else
            {
                MessageBox.Show("Invalid report ID.");
            }
        }

        private void UpdateAcceptedStatus(int reportId, int acceptedStatus)
        {
        
            string query = "UPDATE report SET ACCEPTED = :acceptedStatus WHERE REPORT_ID = :reportId";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(":acceptedStatus", OracleDbType.Int32).Value = acceptedStatus;
                cmd.Parameters.Add(":reportId", OracleDbType.Int32).Value = reportId;

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Status updated successfully."+acceptedStatus);
                        using (OracleTransaction transaction = conn.BeginTransaction())
                        {
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                           transaction.Commit();
                         }
                    }
                    else
                    {
                        MessageBox.Show("Report not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
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

        private void button1_Click(object sender, EventArgs e)
        {
            showTable();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
