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
    public partial class GuestPage : UserControl
    {
        function fn = new function();
        string query;
        public GuestPage()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e) // Add button
        {
            //adding items to the database

            query = "insert into guests (guest_name,guest_address,guest_phonenumber,guest_email) values ('" + textBox1.Text + "','" + textBox2.Text + "','"+ textBox3.Text+"','"+ textBox4.Text+"')";
            fn.SetData(query);
            LoadData();
        }

        private void GuestPage_Load(object sender, EventArgs e)
        {
            // Disable auto-generate columns
            dataGridView1.AutoGenerateColumns = false;

            //// Clear existing columns
            dataGridView1.Columns.Clear();

            //// Define your custom columns
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = "id" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "name", HeaderText = "Name", DataPropertyName = "guest_name" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "address", HeaderText = "Address", DataPropertyName = "guest_address" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "phonenumber", HeaderText = "Phone Number", DataPropertyName = "guest_phonenumber" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "email", HeaderText = "Email", DataPropertyName = "guest_email" });

            // Load data from the database
            string query = "select * from guests";
            DataSet ds = fn.GetData(query);

            // Set the data source
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click is on a valid row (not header row)
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // Extract the values and set them to the text boxess
                textBox1.Text = selectedRow.Cells["name"].Value.ToString();
                textBox2.Text = selectedRow.Cells["address"].Value.ToString();
                textBox3.Text = selectedRow.Cells["phonenumber"].Value.ToString();
                textBox4.Text = selectedRow.Cells["email"].Value.ToString();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string address = textBox2.Text;
            string phoneNumber = textBox3.Text;
            string email = textBox4.Text;
            // Create a query to delete the record based on selected values
            string query = $"DELETE FROM guests WHERE guest_name = '{name}' AND guest_address = '{address}' AND guest_phonenumber = '{phoneNumber}' AND guest_email = '{email}'";

            // Execute the delete query
            fn.SetData(query);

            // Reload data into the DataGridView to reflect the changes
            LoadData();

            // Clear text boxes after deletion
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";

            MessageBox.Show("Record deleted successfully.");

        }
        //private void GuestPage_Load(object sender, EventArgs e)
        //{
        //    query = "select * from guests";
        //    DataSet ds = fn.GetData(query);
        //    dataGridView1.DataSource = ds.Tables[0];

        //}


        private void LoadData()
        {
            // Load data from the database
            string query = "SELECT * FROM guests";
            DataSet ds = fn.GetData(query);

            // Set the data source
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            // Get values from text boxes
            string name = textBox1.Text;
            string address = textBox2.Text;
            string phoneNumber = textBox3.Text;
            string email = textBox4.Text;

            // Create a query to update the record based on selected values
            string query = $"UPDATE guests SET guest_name = '{name}', guest_address = '{address}', guest_phonenumber = '{phoneNumber}', guest_email = '{email}' " +
                "WHERE guest_name = '{name}' AND guest_address = '{address}' AND guest_phonenumber = '{phoneNumber}' AND guest_email = '{email}'";

            // Execute the update query
            fn.SetData(query);

            // Reload data into the DataGridView to reflect the changes
            LoadData();

            MessageBox.Show("Record updated successfully.");
        }
    }
}
