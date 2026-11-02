using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
namespace PromVesClient
{
    public partial class MediaWeighingForm : Form
    {

        public MediaWeighingForm()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void MediaWeighingForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = @"C:\Users\User\Desktop\Сервер\PromVesServer\PromVesServer\bin\Release\net10.0\cameraVideo\Camera1\20261030_050631_856_1.mp4";

            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Видеофайл не найден:\n" + path);
            }
        }
    }
}
