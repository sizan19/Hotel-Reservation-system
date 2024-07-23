using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseAssignment
{
    public partial class FormHomePage : Form
    {
        private string userRole;

        public FormHomePage(string role)
        {
            InitializeComponent();
            userRole = role;
        }

        private void FormHomePage_Load(object sender, EventArgs e)
        {
            label1.Text = userRole;
            homePageUc1.Visible = true;
            homePageUc1.BringToFront();
            guestPage1.Visible = false;
            PictureBack.Visible = false;
            roomPageUc1.Visible = false;
            activityGuest1.Visible = false;

            //make user role only access to Manage button
            if (userRole == "admin")
            {
                button4.Visible = true;
            }
            else
            {
                button4.Visible = false;
            }


        }
        private void button2_Click(object sender, EventArgs e) //Homepage Pressed
        {
            guestPage1.Visible = true;
            guestPage1.BringToFront();
            button2.BackColor = Color.Teal;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            homePageUc1.Visible = true;
            homePageUc1.BringToFront();
            button1.BackColor = Color.Teal;


        }

        private void pictureBack_Click(object sender, EventArgs e)
        {
            homePageUc1.Visible = true;
            homePageUc1.BringToFront();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            roomPageUc1.Visible = true;
            roomPageUc1.BringToFront();
            button4.BackColor = Color.Teal;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            activityGuest1.Visible = true;
            activityGuest1.BringToFront();

        }

    }
}
