using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseAssignment
{

    public partial class Form1 : Form
    {
        function fn = new function();
        string query;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            query = "SELECT * FROM Login WHERE username = '"+ textBox1.Text + "' AND password = '"+ textBox2.Text + "'";
            DataSet ds = fn.GetData(query);
            Console.WriteLine(ds);
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string role = ds.Tables[0].Rows[0]["role"].ToString().Trim(); // Trim the role text

                    //Console.WriteLine("Role: " + role); 

                    Form f2 = new FormHomePage(role);
                    f2.Show();
                    string discordMessage = $"{role} {textBox1.Text} logged into the system.";
                    SendMs(discordMessage);
                }
                else
                {
                    MessageBox.Show("You are not granted access.");
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
