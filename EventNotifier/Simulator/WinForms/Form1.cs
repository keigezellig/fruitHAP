using System;
using System.Windows.Forms;
using SimulatorCommon;

namespace WinForms
{
    public partial class Form1 : Form
    {
        private string imagePath;
        
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

            SendMessage(connectionString,imagePath);
        }

        private void SendMessage(string connectionString, string imagePath)
        {
            try
            {
                SimulatorLogic.PublishRingMessage(connectionString, imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error occured! {0}", ex));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openImageDialog = new OpenFileDialog();
            openImageDialog.Filter = "Image files|*.jpg;*.bmp;*.png";
            openImageDialog.CheckFileExists = true;

            if (openImageDialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = openImageDialog.FileName;
                pictureBox1.ImageLocation = imagePath;
            }
        }
    }
}
