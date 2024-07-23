using System;
using System.Collections;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DatabaseAssignment
{
    public partial class UcCheckOut : UserControl
    {
        function fn = new function();
        string query;
        public UcCheckOut()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            // Create the query to fetch guest names where guest_id exists in guest_rooms
            string query = $"SELECT g.guest_name " +
                           $"FROM guests g " +
                           $"INNER JOIN guest_rooms gr ON g.id = gr.guest_id " +
                           $"WHERE g.guest_name LIKE '{textBox1.Text}%'";

            // Assuming fn is the instance of the class containing GetData method
            DataSet ds = fn.GetData(query);

            // Populate the listBox with the guest names
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listBox1.Items.Add(row["guest_name"].ToString());
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Clear();
            string text = listBox1.GetItemText(listBox1.SelectedItem);

            // Query to get the guest ID
            string guestIdQuery = "SELECT Id FROM guests WHERE guest_name = '" + text + "'";
            DataSet dsGuest = fn.GetData(guestIdQuery);

            if (dsGuest.Tables[0].Rows.Count > 0)
            {
                // Set the guest ID in textBox2
                int guestId = Convert.ToInt32(dsGuest.Tables[0].Rows[0]["Id"]);
                textBox2.Text = guestId.ToString();

                // Query to get the room ID from guest_rooms using the guest ID
                string roomIdQuery = "SELECT room_id FROM guest_rooms WHERE guest_id = " + guestId;
                DataSet dsRoom = fn.GetData(roomIdQuery);

                if (dsRoom.Tables[0].Rows.Count > 0)
                {
                    int roomId = Convert.ToInt32(dsRoom.Tables[0].Rows[0]["room_id"]);
                    // Assuming you want to display room ID in a text box or perform some other action with it
                    // For example, if you have a textBox3 for displaying room ID:
                    textBox2.Text = roomId.ToString();
                }
                else
                {
                    MessageBox.Show("No room found for the selected guest.");
                }
            }
            else
            {
                MessageBox.Show("Guest not found.");
            }
        }

        private void button1_Click(object sender, EventArgs e) ///checkout button
        {
            string roomId = textBox2.Text; // Assuming textBox2 contains room_id

            // Get current date for checkout_date
            DateTime checkoutDate = DateTime.Today;

            // Query to fetch guest_id from guest_rooms using the room_id
            string guestIdquery = $"SELECT guest_id FROM guest_rooms WHERE room_id = {roomId}";
            DataSet dsGuestId = fn.GetData(guestIdquery);
            int guestId = Convert.ToInt32(dsGuestId.Tables[0].Rows[0]["guest_id"]);

            // To get guestName here
            string guestName = string.Empty;
            string guestNameQuery = $"SELECT guest_name FROM guests WHERE Id = {guestId}";
            DataSet dsGuestName = fn.GetData(guestNameQuery);

            guestName = dsGuestName.Tables[0].Rows[0]["guest_name"].ToString();
            string trimmedguestName = guestName.Trim();  // Removes leading and trailing whitespace

            // Query to fetch all room_ids associated with the guest
            string roomIdsQuery = $"SELECT room_id FROM guest_rooms WHERE guest_id = {guestId} AND checkin_date IS NOT NULL";
            DataSet dsRoomIds = fn.GetData(roomIdsQuery);

            if (dsRoomIds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dsRoomIds.Tables[0].Rows)
                {
                    string currentRoomId = row["room_id"].ToString();

                    // Query to fetch room_price from rooms table
                    string roomPriceQuery = $"SELECT room_price FROM rooms WHERE id = {currentRoomId}";
                    DataSet dsRoomPrice = fn.GetData(roomPriceQuery);

                    if (dsRoomPrice.Tables[0].Rows.Count > 0)
                    {
                        // Retrieve room_price from the DataSet
                        decimal roomPrice = Convert.ToDecimal(dsRoomPrice.Tables[0].Rows[0]["room_price"]);

                        // Query to update guest_rooms with checkout_date, total_room_price, and room_status
                        string updateGuestRoomsQuery = $@"
                    UPDATE guest_rooms
                    SET checkout_date = '{checkoutDate:yyyy-MM-dd}', 
                        total_room_price = 
                            CASE 
                                WHEN DATEDIFF(day, checkin_date, '{checkoutDate:yyyy-MM-dd}') = 0 
                                THEN {roomPrice} 
                                ELSE DATEDIFF(day, checkin_date, '{checkoutDate:yyyy-MM-dd}') * {roomPrice}
                            END
                    WHERE room_id = {currentRoomId} AND checkin_date IS NOT NULL";

                        // Execute the query to update guest_rooms
                        fn.SetData(updateGuestRoomsQuery);

                        // Query to update room_status in rooms table
                        string updateRoomStatusQuery = $"UPDATE rooms SET room_status = 'Free' WHERE id = {currentRoomId}";

                        // Execute the query to update room_status
                        fn.SetData(updateRoomStatusQuery);
                    }
                }

                string discordMessage = $"Checkout completed for {trimmedguestName} for all associated rooms.";
                SendMs(discordMessage);

                DisplayUpdatedData(guestId);

                MessageBox.Show("Checkout completed successfully for all rooms.");
            }
            else
            {
                MessageBox.Show($"No rooms found for guest with ID {guestId}.");
            }
        }

        private void DisplayUpdatedData(int guestId)
        {
            try
            {
                // Query to fetch guest_name and other data from guest_rooms and guests tables
                string fetchRoomsQuery = $@"
        SELECT gr.Id AS guest_room_id, g.guest_name, gr.room_id, gr.checkin_date, gr.checkout_date, gr.total_room_price
        FROM guest_rooms gr
        INNER JOIN guests g ON gr.guest_id = g.Id
        WHERE gr.guest_id = {guestId}";

                // Execute the query to fetch data from guest_rooms and guests
                DataSet dsRooms = fn.GetData(fetchRoomsQuery);

                // Query to fetch activities data along with activity names from guest_activity and activities tables
                string fetchActivitiesQuery = $@"
        SELECT ga.Id AS guest_activity_id, ga.total_activity_price, a.activity_name
        FROM guest_activities ga
        INNER JOIN activities a ON ga.activity_id = a.Id
        WHERE ga.guest_id = {guestId}";

                // Execute the query to fetch data from guest_activity and activities
                DataSet dsActivities = fn.GetData(fetchActivitiesQuery);

                // Check if room data is fetched correctly
                if (dsRooms == null || dsRooms.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("No room data found for the specified guest.");
                    return;
                }

                // Create a DataTable to store formatted data
                DataTable dt = new DataTable();
                dt.Columns.Add("S.N", typeof(int));
                dt.Columns.Add("Description", typeof(string));
                dt.Columns.Add("Price", typeof(decimal));

                int serialNumber = 1;
                decimal totalRoomPriceSum = 0;
                decimal totalActivityPriceSum = 0;

                // Arrays to hold IDs
                List<int> guestRoomIds = new List<int>();
                List<int> guestActivityIds = new List<int>();

                // Add rows from guest_rooms data
                foreach (DataRow row in dsRooms.Tables[0].Rows)
                {
                    int roomId = Convert.ToInt32(row["room_id"]);
                    int guestRoomId = Convert.ToInt32(row["guest_room_id"]);
                    DateTime checkinDate = Convert.ToDateTime(row["checkin_date"]);
                    DateTime checkoutDate = Convert.ToDateTime(row["checkout_date"]);
                    decimal totalRoomPrice = Convert.ToDecimal(row["total_room_price"]);

                    // Calculate duration
                    TimeSpan duration = checkoutDate - checkinDate;

                    // Create the description
                    string description = $"Stayed in Room {roomId} - {duration.Days} days";

                    // Add the row to the DataTable
                    dt.Rows.Add(serialNumber++, description, totalRoomPrice);

                    // Add to the total room price sum
                    totalRoomPriceSum += totalRoomPrice;

                    // Add guest_room_id to the list
                    guestRoomIds.Add(guestRoomId);
                }

                // Add rows from guest_activity data if available
                if (dsActivities != null && dsActivities.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dsActivities.Tables[0].Rows)
                    {
                        decimal totalActivityPrice = Convert.ToDecimal(row["total_activity_price"]);
                        string activityName = row["activity_name"].ToString();
                        int guestActivityId = Convert.ToInt32(row["guest_activity_id"]);
                        string activityDescription = $"Activity: {activityName}";

                        // Add the activity row to the DataTable
                        dt.Rows.Add(serialNumber++, activityDescription, totalActivityPrice);

                        // Add to the total activity price sum
                        totalActivityPriceSum += totalActivityPrice;

                        // Add guest_activity_id to the list
                        guestActivityIds.Add(guestActivityId);
                    }
                }

                // Bind the DataTable to the DataGridView
                dataGridView1.DataSource = dt;

                // Calculate the total price and update label4
                decimal totalPrice = totalRoomPriceSum + totalActivityPriceSum;
                label4.Text = $"Total Price: {totalPrice}";

                // Insert payment data
                InsertPaymentData(totalPrice, DateTime.Now, comboBox1.SelectedItem?.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching data: {ex.Message}");
            }
        }

        private void InsertPaymentData(decimal totalAmountPrice, DateTime paymentDate, string paymentType)
        {
            try
            {
                string insertQuery = $@"
                INSERT INTO payment (total_amount_price, payment_date, payment_type)
                VALUES ('{totalAmountPrice}', '{paymentDate}', '{paymentType}')";

                fn.SetData(insertQuery);

                MessageBox.Show("Payment data inserted successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while inserting data: {ex.Message}");
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
