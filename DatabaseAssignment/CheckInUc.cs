using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseAssignment
{
    public partial class CheckInUc : UserControl
    {
        function fn = new function();
        string query;
        public CheckInUc()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //check in button pressed
        {
            // Retrieve the text values from the textboxes and combobox
            string selectedGuestName = textBox2.Text.Trim();
            string selectedGuestNumber = textBox3.Text.Trim();
            string selectedRoomId = comboBox1.SelectedItem.ToString(); // Use SelectedValue to get the actual value

            // Get current date in yyyy-MM-dd format
            string currentDate = DateTime.Today.ToString("yyyy-MM-dd");

            // Correct the SQL query to use the retrieved values
            string fetchIdQuery = $"SELECT Id FROM guests WHERE guest_name = '{selectedGuestName}' AND guest_phonenumber = '{selectedGuestNumber}'";
            DataSet ds = fn.GetData(fetchIdQuery);

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                int guestId = Convert.ToInt32(row["Id"]);

                // Create the query to insert into guest_rooms
                string insertQuery = $"INSERT INTO guest_rooms (guest_id, room_id, checkin_date) VALUES ({guestId}, {selectedRoomId}, '{currentDate}')";
                fn.SetData(insertQuery);

                // Create the query to update the room status to "Occupied"
                string updateRoomStatusQuery = $"UPDATE rooms SET room_status = 'Occupied' WHERE id = {selectedRoomId}";
                fn.SetData(updateRoomStatusQuery);
                string discordMessage = $"{selectedGuestName} sucessfully checked in to {selectedRoomId}.";
                SendMs(discordMessage);

                MessageBox.Show("Guest check-in successful and room status updated to Occupied.");
            }
            else
            {
                // Handle the case where no guest was found
                MessageBox.Show("Guest not found. Please check the details and try again.");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            // Define your query to retrieve id from the rooms table where room_status is free
            string query = "SELECT id FROM rooms WHERE room_status = 'free'";

            // Retrieve data from the database
            DataSet ds = fn.GetData(query);

            // Check if the dataset has tables and rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // Loop through the DataSet and add each id to the ComboBox
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    comboBox1.Items.Add(row["id"].ToString());
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
            {
                textBox2.Clear();
                textBox3.Clear();
                string text = listBox1.GetItemText(listBox1.SelectedItem);
                textBox2.Text = text;
                query = "select guest_phonenumber from guests where guest_name = '" + text + "'";
                DataSet ds = fn.GetData(query);
                textBox3.Text = ds.Tables[0].Rows[0][0].ToString();  //adding selected items to the quantity changing grid
            }
        }

        private void comboBox2_Enter(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();

            // Define your query to retrieve id from the rooms table where room_status is free
            string query = "SELECT type_name FROM roomtype";

            // Retrieve data from the database
            DataSet ds = fn.GetData(query);

            // Check if the dataset has tables and rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // Loop through the DataSet and add each id to the ComboBox
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    comboBox2.Items.Add(row["type_name"].ToString());
                }
            }

        }

        static void SendMs(string message)
        {
            string webhook = "https://discord.com/api/webhooks/1265355785467199548/NAy7dnITVm-yfiXUqNJoQ4vAYuYFSB1vUWSlJtfSrffp9ZSvBZ8Sy5kLmKECcNAeQIyM";

            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            string payload = "{\"content\": \"" + message + "\"}";
            client.UploadData(webhook, Encoding.UTF8.GetBytes(payload));
        }
    }
}
