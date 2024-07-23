using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseAssignment
{
    public partial class HomePageUc : UserControl
    {
        public HomePageUc()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            checkInUc1.Visible = true;
            checkInUc1.BringToFront();
        }

        private void HomePageUc_Load(object sender, EventArgs e)
        {
            checkInUc1.Visible=false;
            ucCheckOut1.Visible=false;
        }

        private void HomePageUc_Leave(object sender, EventArgs e)
        {
            checkInUc1.Visible=false;
            ucCheckOut1.Visible = false;


        }

        private void checkInUc1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ucCheckOut1.Visible = true;
            ucCheckOut1.BringToFront();


        }
    }
}
