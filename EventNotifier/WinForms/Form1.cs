using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimulatorCommon;

namespace WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = txtMessageServer.Text;

            SendMessage(connectionString);
        }

        private static void SendMessage(string connectionString)
        {
            try
            {
               SimulatorLogic.PublishRingMessage(connectionString,null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error occured! {0}", ex.Message));
            }
        }
    }
}
