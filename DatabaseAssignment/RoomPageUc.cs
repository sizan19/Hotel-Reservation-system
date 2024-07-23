using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DatabaseAssignment
{
    public partial class RoomPageUc : UserControl
    {
        function fn = new function();
        string query;
        public RoomPageUc()
        {
            InitializeComponent();
        }



        private void LoadData()
        {
            // Load data from the database
            string query = "SELECT * FROM roomtype";
            DataSet ds = fn.GetData(query);

            // Set the data source
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //adding items to the database

            query = "insert into roomtype (id,type_name,type_price,type_capacity) values ('" + textBox8.Text + "','" + textBox6.Text + "','" + textBox7.Text + "','" + numericUpDown1.Value + "')";
            fn.SetData(query);
            LoadData();
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;

            // Clear existing columns
            dataGridView1.Columns.Clear();

            // Define your custom columns
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = "id" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "name", HeaderText = "Name", DataPropertyName = "type_name" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "price", HeaderText = "Price", DataPropertyName = "type_price" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "capacity", HeaderText = "Capacity", DataPropertyName = "type_capacity" });

            // Load data from the database
            string query = "select * from roomtype";
            DataSet ds = fn.GetData(query);

            // Set the data source
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // Extract the values and set them to the text boxess
                textBox8.Text = selectedRow.Cells["id"].Value.ToString();
                textBox6.Text = selectedRow.Cells["name"].Value.ToString();
                textBox7.Text = selectedRow.Cells["price"].Value.ToString();
                //numericUpDown1.Value = selectedRow.Cells["email"].Value();
            }
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            // Clear existing items to avoid duplication
            comboBox1.Items.Clear();

            // Define your query to retrieve type_name from the roomtype table
            string query = "SELECT type_name FROM roomtype";

            // Retrieve data from the database
            DataSet ds = fn.GetData(query);

            // Check if the dataset has tables and rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // Loop through the DataSet and add each type_name to the ComboBox
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    comboBox1.Items.Add(row["type_name"].ToString());
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            // Ensure that an item is selected in the type ComboBox
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a room type.");
                return;
            }

            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a room status.");
                return;
            }

            string selectedTypeName = comboBox1.SelectedItem.ToString();
            string roomStatus = comboBox2.SelectedItem.ToString();

            string fetchQuery = $"SELECT id, type_price FROM roomtype WHERE type_name = '{selectedTypeName}'";

            // Fetch type_id and type_price
            DataSet ds = fn.GetData(fetchQuery);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                int typeId = Convert.ToInt32(row["id"]);
                decimal typePrice = Convert.ToDecimal(row["type_price"]);

                // Get the values for id from text box or other input control
                string id = textBox3.Text; // Assuming textBox3 contains the room ID

                // Define a query to insert the new record into the room table
                string insertQuery = $"INSERT INTO rooms (id, room_status, room_price, type_id) " +
                                     $"VALUES ('{id}', '{roomStatus}', '{typePrice}', '{typeId}')";

                // Insert the new record into the room table
                fn.SetData(insertQuery);

                MessageBox.Show("Room record added successfully.");
                LoadDataForSecond();
            }
            else
            {
                MessageBox.Show("No room type found with the selected name.");
            }
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {

            dataGridView2.AutoGenerateColumns = false;

            // Clear existing columns
            dataGridView2.Columns.Clear();

            // Define your custom columns
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = "id" });
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "Status", DataPropertyName = "room_status" });
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn { Name = "price", HeaderText = "Price", DataPropertyName = "room_price" });
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn { Name = "typeid", HeaderText = "typeID", DataPropertyName = "type_id" });

            // Load data from the database
            string query = "select * from rooms";
            DataSet ds = fn.GetData(query);

            // Set the data source
            dataGridView2.DataSource = ds.Tables[0];
            dataGridView2.CellFormatting += dataGridView2_CellFormatting;


        }


        private void LoadDataForSecond()
        {
            // Load data from the database
            string query = "SELECT * FROM rooms";
            DataSet ds = fn.GetData(query);

            // Set the data source
            dataGridView2.DataSource = ds.Tables[0];
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Check the column name to apply formatting conditionally
                if (dataGridView2.Columns[e.ColumnIndex].Name == "status") // Assuming "name" column is room_status
                {
                    string roomStatus = e.Value?.ToString() ?? string.Empty;
                    // Set background color based on room_status
                    if (roomStatus.ToLower() == "free")
                    {
                        dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else if (roomStatus.ToLower() == "occupied")
                    {
                        dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                    }
                    else
                    {
                        // Reset to default if neither "free" nor "occupied"
                        dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e) //deleting rooms from roomtable
        {
            string id = textBox3.Text;
            // Create a query to delete the record based on selected values
            string query = $"DELETE FROM rooms WHERE id = '{id}'";

            // Execute the delete query
            fn.SetData(query);

            // Reload data into the DataGridView to reflect the changes
            LoadData();

            // Clear text boxes after deletion
            textBox3.Text = "";

            MessageBox.Show("Record deleted successfully.");
        }

        private void dataGridView2_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];

                // Extract the values and set them to the text boxess
                textBox3.Text = selectedRow.Cells["Id"].Value.ToString();
            }

        }

        private void button7_Click(object sender, EventArgs e) //Activity Button
        {
            //adding activity to database
            query = "insert into activities (activity_name,activity_price) values ('" + textBox1.Text + "','" + textBox2.Text + "')";
            fn.SetData(query);
            LoadActivityData();
        }

        //Show activity to grid
        private void LoadActivityData()
        {
            // Load data from the database
            string query = "SELECT * FROM activities";
            DataSet ds = fn.GetData(query);

            // Set the data source
            dataGridView3.DataSource = ds.Tables[0];
        }

        private void button8_Click(object sender, EventArgs e) //Activity button (edit)
        {
            // Get values from text boxes
            string activityname = textBox1.Text;
            decimal activityprice;
            decimal.TryParse(textBox2.Text, out activityprice);

            // Create a query to update the record based on selected values
            string activityedit = $"UPDATE activities SET activity_price = '{activityprice}' WHERE activity_name = '{activityname}'";

            // Execute the update query
            fn.SetData(activityedit);

            // Reload data into the DataGridView to reflect the changes
            LoadActivityData();

            MessageBox.Show("Record updated successfully.");

        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridView3.Rows[e.RowIndex];

                // Extract the values and set them to the text boxess
                textBox1.Text = selectedRow.Cells["activity_name"].Value.ToString();
                textBox2.Text = selectedRow.Cells["activity_price"].Value.ToString();

            }
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            LoadActivityData();

        }






        /// <summary>
        /// For User Page
        /// </summary>


        private void LoadUserData()
        {
            // Load data from the database
            string query = "SELECT * FROM Login";
            DataSet ds = fn.GetData(query);

            // Set the data source
            dataGridView4.DataSource = ds.Tables[0];
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string role = comboBox3.SelectedItem.ToString(); // Get selected role
            //adding activity to database
            query = "insert into Login (username,password,role) values ('" + textBox4.Text + "','" + textBox5.Text + "','"+ role +"')";
            fn.SetData(query);
            LoadUserData();

        }

        private void tabPage4_Enter(object sender, EventArgs e)
        {
            LoadUserData();
        }

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridView4.Rows[e.RowIndex];

                // Extract the values and set them to the text boxess
                textBox4.Text = selectedRow.Cells["username"].Value.ToString();
                textBox5.Text = selectedRow.Cells["password"].Value.ToString();

            }

        }

        private void button12_Click(object sender, EventArgs e) ///Delete Users//
        {
            string username = textBox4.Text;
            string password = textBox5.Text;

            // Create a query to delete the record based on selected values
            string query = $"DELETE FROM Login WHERE username = '{username}' and password = '{password}'";

            // Execute the delete query
            fn.SetData(query);

            // Reload data into the DataGridView to reflect the changes
            LoadUserData();

            // Clear text boxes after deletion
            textBox4.Text = "";
            textBox5.Text = "";
            MessageBox.Show("Record deleted successfully.");
        }

        private void button11_Click(object sender, EventArgs e) //Edit Users password and role//
        {
            // Get values from text boxes
            string username = textBox4.Text.Trim();
            string password = textBox5.Text.Trim();
            string role = comboBox3.SelectedItem.ToString(); // Get selected role


            // Create a query to update the record based on selected values
            string UserEdit = $"UPDATE Login SET password = '{password}' and role = '{role}' WHERE username = '{username}'";

            // Execute the update query
            fn.SetData(UserEdit);

            // Reload data into the DataGridView to reflect the changes
            LoadActivityData();

            MessageBox.Show("Record updated successfully.");

        }






        ///////////report section///////////
        ///
        private void DisplayTodaysData()
        {
            // Get today's date
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            // Query to get the number of rooms occupied today
            string fetchOccupiedRoomsQuery = $@"
        SELECT COUNT(*) AS OccupiedRooms
        FROM guest_rooms
        WHERE checkin_date = '{today}'";

            // Execute the query to get occupied rooms
            DataSet dsOccupiedRooms = fn.GetData(fetchOccupiedRoomsQuery);
            int occupiedRooms = 0;
            if (dsOccupiedRooms.Tables[0].Rows.Count > 0)
            {
                occupiedRooms = Convert.ToInt32(dsOccupiedRooms.Tables[0].Rows[0]["OccupiedRooms"]);
            }

            // Query to get the total number of rooms
            string fetchTotalRoomsQuery = "SELECT COUNT(*) AS TotalRooms FROM rooms";

            // Execute the query to get total rooms
            DataSet dsTotalRooms = fn.GetData(fetchTotalRoomsQuery);
            int totalRooms = 0;
            if (dsTotalRooms.Tables[0].Rows.Count > 0)
            {
                totalRooms = Convert.ToInt32(dsTotalRooms.Tables[0].Rows[0]["TotalRooms"]);
            }

            // Display today's room occupancy
            string occupancyText = $"{occupiedRooms} / {totalRooms}";
            label19.Text = occupancyText; // Assuming labelTodaysRoom is the label in the group box for today's room occupancy
        }

        private void DisplayWeeklyData()
        {
            // Get today's date and the date one week ago
            DateTime today = DateTime.Today;
            DateTime oneWeekAgo = today.AddDays(-6); // Including today, hence -6

            // Query to get the room occupancy for the past week
            string fetchWeeklyRoomsQuery = $@"
        SELECT CAST(checkin_date AS DATE) AS CheckinDate, COUNT(*) AS OccupiedRooms
        FROM guest_rooms
        WHERE checkin_date BETWEEN '{oneWeekAgo:yyyy-MM-dd}' AND '{today:yyyy-MM-dd}'
        GROUP BY CAST(checkin_date AS DATE)
        ORDER BY CheckinDate";

            // Execute the query to fetch weekly room occupancy data
            DataSet dsWeeklyRooms = fn.GetData(fetchWeeklyRoomsQuery);

            // Prepare a DataTable to ensure every date in the range is represented
            DataTable dt = new DataTable();
            dt.Columns.Add("CheckinDate", typeof(DateTime));
            dt.Columns.Add("OccupiedRooms", typeof(int));

            // Populate the DataTable with all dates within the range
            for (DateTime date = oneWeekAgo; date <= today; date = date.AddDays(1))
            {
                DataRow row = dt.NewRow();
                row["CheckinDate"] = date;
                row["OccupiedRooms"] = 0; // Initialize with 0 occupancy
                dt.Rows.Add(row);
            }

            // Merge the actual data into this DataTable
            foreach (DataRow dataRow in dsWeeklyRooms.Tables[0].Rows)
            {
                DateTime checkinDate = Convert.ToDateTime(dataRow["CheckinDate"]);
                int occupiedRooms = Convert.ToInt32(dataRow["OccupiedRooms"]);

                // Find the row with the matching date and update the occupancy
                DataRow[] rowsToUpdate = dt.Select($"CheckinDate = #{checkinDate:yyyy-MM-dd}#");
                if (rowsToUpdate.Length > 0)
                {
                    rowsToUpdate[0]["OccupiedRooms"] = occupiedRooms;
                }
            }

            // Set up the bar diagram
            ChartOccupancy.Series.Clear();
            ChartOccupancy.ChartAreas.Clear();

            var series = new Series
            {
                Name = "WeeklyOccupancy",
                IsVisibleInLegend = false,
                ChartType = SeriesChartType.Column // Set the chart type to Column for a bar diagram
            };
            ChartOccupancy.Series.Add(series);

            // Bind the chart to the DataTable
            ChartOccupancy.DataSource = dt;

            // Set X and Y values
            series.XValueMember = "CheckinDate";
            series.YValueMembers = "OccupiedRooms";

            // Customize the chart area
            ChartArea chartArea = new ChartArea();
            ChartOccupancy.ChartAreas.Add(chartArea);
            chartArea.AxisY.Title = "Occupied Rooms";
            chartArea.AxisY.Minimum = 0;

            ChartOccupancy.Invalidate(); // Redraw the chart
        }

        private void DisplayWeeklySales()
        {
            DateTime today = DateTime.Today;
            DateTime sevenDaysAgo = today.AddDays(-6); // 7 days including today

            string query = $@"
        SELECT CAST(payment_date AS DATE) AS PaymentDate, ISNULL(SUM(total_amount_price), 0) AS TotalSales
        FROM payment
        WHERE CAST(payment_date AS DATE) BETWEEN '{sevenDaysAgo:yyyy-MM-dd}' AND '{today:yyyy-MM-dd}'
        GROUP BY CAST(payment_date AS DATE)
        ORDER BY PaymentDate";

            DataSet dsSales = fn.GetData(query);

            DataTable dt = new DataTable();
            dt.Columns.Add("PaymentDate", typeof(DateTime));
            dt.Columns.Add("TotalSales", typeof(decimal));

            for (DateTime date = sevenDaysAgo; date <= today; date = date.AddDays(1))
            {
                dt.Rows.Add(date, 0m);
            }

            foreach (DataRow row in dsSales.Tables[0].Rows)
            {
                DateTime date = Convert.ToDateTime(row["PaymentDate"]);
                decimal sales = Convert.ToDecimal(row["TotalSales"]);
                DataRow[] rows = dt.Select($"PaymentDate = '{date:yyyy-MM-dd}'");
                if (rows.Length > 0) rows[0]["TotalSales"] = sales;
            }

            chart2.Series.Clear();
            chart2.ChartAreas.Clear();
            var series = new Series { ChartType = SeriesChartType.Column };
            chart2.Series.Add(series);
            chart2.DataSource = dt;
            series.XValueMember = "PaymentDate";
            series.YValueMembers = "TotalSales";

            ChartArea chartArea = new ChartArea
            {
                AxisX = { Title = "Date", LabelStyle = { Format = "MM/dd" } },
                AxisY = { Title = "Total Sales", Minimum = 0 }
            };
            chart2.ChartAreas.Add(chartArea);

            // Hide the legend
            chart2.Legends.Clear();

            chart2.DataBind();
            chart2.Invalidate();
        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            chart2.Visible = false;
            ChartOccupancy.Visible = false;
            DisplayWeeklyData();
            DisplayTodaysData();
            DisplayWeeklySales();
            progressBar1.Visible = false;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string roomId = textBox3.Text.Trim();
            string status = comboBox2.SelectedItem.ToString().Trim();

            if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Update the room status in the database
            string query = "UPDATE Rooms SET room_status = '" + status + "' WHERE Id = '" + roomId + "'";
            fn.SetData(query);
            LoadDataForSecond();

            // Optionally clear the input fields after updating
            textBox3.Clear();
            comboBox2.SelectedIndex = 0; // Reset to default selection
        }

        private async void button13_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = 0;

            var progress = new Progress<int>(value =>
            {
                progressBar1.Value = value;
            });

            await PerformTaskAsync(progress);

            progressBar1.Visible = false;
            chart2.Visible = true;
            ChartOccupancy.Visible = true;
        }

        private async Task PerformTaskAsync(IProgress<int> progress)
        {
            for (int i = 0; i <= 100; i++)
            {
                await Task.Delay(30); // Simulate work being done
                progress.Report(i);
            }
        }
    }
}
