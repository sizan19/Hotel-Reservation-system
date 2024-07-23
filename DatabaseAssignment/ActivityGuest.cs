using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DatabaseAssignment
{
    public partial class ActivityGuest : UserControl
    {
        function fn = new function();
        string query;
        public ActivityGuest()
        {
            InitializeComponent();
        }

        private void comboBox2_Enter(object sender, EventArgs e) //fill up activity with activity_name
        {
            comboBox2.Items.Clear();

            // Define your query to retrieve id from the rooms table where room_status is free
            string query = "SELECT activity_name FROM activities";

            // Retrieve data from the database
            DataSet ds = fn.GetData(query);

            // Check if the dataset has tables and rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // Loop through the DataSet and add each id to the ComboBox
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    comboBox2.Items.Add(row["activity_name"].ToString());
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            {
                listBox1.Items.Clear();
                query = "select guest_name from guests where guest_name like'" + textBox1.Text + "%'";
                DataSet ds = fn.GetData(query);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    listBox1.Items.Add(ds.Tables[0].Rows[i][0].ToString());     //putting the search result that starts with letter 
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox3.Clear();
            string text = listBox1.GetItemText(listBox1.SelectedItem);
            textBox2.Text = text;
            query = "select guest_phonenumber from guests where guest_name = '" + text + "'";
            DataSet ds = fn.GetData(query);
            textBox3.Text = ds.Tables[0].Rows[0][0].ToString();  //adding selected items to the quantity changing grid
        }

        private void button1_Click(object sender, EventArgs e) //add activity to guest
        {
            // Retrieve the text values from the textboxes and combobox
            string selectedGuestName = textBox2.Text.Trim();
            string selectedGuestNumber = textBox3.Text.Trim();
            string selectedActivityName = comboBox2.SelectedItem.ToString().Trim(); // Use SelectedValue to get the actual value

            // Get current date in yyyy-MM-dd format
            string currentDate = DateTime.Today.ToString("yyyy-MM-dd");

            // Correct the SQL query to get activity_id and activity_price
            string activityQuery = $"SELECT Id, activity_price FROM activities WHERE activity_name = '{selectedActivityName}'";
            DataSet dsActivity = fn.GetData(activityQuery);

            if (dsActivity.Tables[0].Rows.Count > 0)
            {
                DataRow activityRow = dsActivity.Tables[0].Rows[0];
                int activityId = Convert.ToInt32(activityRow["Id"]);
                decimal activityPrice = Convert.ToDecimal(activityRow["activity_price"]);

                // Correct the SQL query to use the retrieved values
                string fetchIdQuery = $"SELECT Id FROM guests WHERE guest_name = '{selectedGuestName}' AND guest_phonenumber = '{selectedGuestNumber}'";
                DataSet dsGuest = fn.GetData(fetchIdQuery);

                if (dsGuest.Tables[0].Rows.Count > 0)
                {
                    DataRow guestRow = dsGuest.Tables[0].Rows[0];
                    int guestId = Convert.ToInt32(guestRow["Id"]);

                    // Create the query to insert into guest_activities
                    string insertQuery = $"INSERT INTO guest_activities (guest_id, activity_id, activity_date, total_activity_price) VALUES ({guestId}, {activityId}, '{currentDate}', {activityPrice})";
                    fn.SetData(insertQuery);

                    MessageBox.Show("Activity added to guest.");
                }
                else
                {
                    // Handle the case where no guest was found
                    MessageBox.Show("Guest not found. Please check the details and try again.");
                }
            }
            else
            {
                // Handle the case where no activity was found
                MessageBox.Show("Activity not found. Please check the details and try again.");
            }
        }
    }
}
