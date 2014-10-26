using System;
using System.Windows.Forms;
using DoorPi.MessageQueuePublisher;
using EventNotifierService.Common;
using EventNotifierService.Common.Messages;

namespace WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
                using (IMQPublisher publisher = new RabbitMqPublisher(connectionString))
                {
                    DoorMessage message = new DoorMessage {EventType = EventType.Ring, TimeStamp = DateTime.Now};
                    publisher.Publish(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error occured! {0}", ex));
            }
        }

        private void txtMessageServer_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
