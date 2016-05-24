using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sprite_Splitter
{
    public partial class Form1 : Form
    {
        Rectangle[] rectangles = new Rectangle[0];
        string filename = Application.StartupPath + "\\Ken2.png";
        Size scanSize = new Size(150, 150);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IWidth.Maximum = 151;
            IHeight.Maximum = 151;
            IWidth.Value = scanSize.Width;
            IHeight.Value = scanSize.Height;
            OpenImage(filename);
            textBox1.Text = filename;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "All Images|*.jpg;*.png;*.jpeg;*.bmp;*.tiff;*.gif|All Files|*.*";
            if(open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = open.FileName;
                OpenImage(filename);
                textBox1.Text = filename;
            }
        }

        private void OpenImage(string filename)
        {
            scanSize.Width = (int)IWidth.Value;
            scanSize.Height = (int)IHeight.Value;
            Bitmap image = (Bitmap)Image.FromFile(filename);
            IWidth.Maximum = image.Width;
            IHeight.Maximum = image.Height;
            SpriteReader reader = new SpriteReader();
            reader.GetTransparentColor(image, 1, 1);
            rectangles = reader.findRectsSprites(image, new Point(0, 0), scanSize);
            pictureBox1.Image = image;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(rectangles.Length > 0) e.Graphics.DrawRectangles(Pens.Black, rectangles);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenImage(filename);
        }
    }
}
