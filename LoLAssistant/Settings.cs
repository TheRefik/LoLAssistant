using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using LoLAssistant.Classes.LiveMatch;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace LoLAssistant
{
    public partial class Settings : MetroForm
    {
        public Settings()
        {
            InitializeComponent();
            Main main = new Main();
            metroStyleManager = main.StyleManager;
            this.Style = metroStyleManager.Style;
            this.Theme = metroStyleManager.Theme;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (metroTextBox1.Text == string.Empty)
            {

            }
            else
            {
                pictureBox1.Load(metroTextBox1.Text);
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = int.Parse(metroTextBox2.Text);
            pictureBox1.Height = int.Parse(metroTextBox3.Text);
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            switch (metroComboBox1.Text)
            {
                case "AutoSize":
                    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                    break;
                case "Zoom":
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    break;
                case "StretchImage":
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    break;
                case "CenterImage":
                    pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                    break;
                case "Normal":
                    pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                    break;
            }
        }
    }
}
