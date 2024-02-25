using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frontend
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            CollapseMenu();
        }

        private void Form5_Load(object sender, EventArgs e)
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
            Form3 form3 = new Form3();
            this.Hide();
            form3.ShowDialog();
        }

        private void iconButtonCheck_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            this.Hide();
            form4.ShowDialog();
        }

        private void iconButtonStats_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            this.Hide();
            form5.ShowDialog();
        }

        private void iconButtonReport_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            this.Hide();
            form6.ShowDialog();
        }

        private void iconButtonAccident_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            this.Hide();
            form7.ShowDialog();
        }

        private void iconButtonReview_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8();
            this.Hide();
            form8.ShowDialog();
        }
    }
}
