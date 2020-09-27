using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
namespace Sobel
{
    public partial class Form1 : Form
    {
        public LogicHandler handler = null;
        public Form1()
        {
            InitializeComponent();
            handler = new LogicHandler();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    handler.FilePath = openFileDialog1.FileName;
                    handler.InputImage = new Image<Bgr, byte>(openFileDialog1.FileName);
                    pictureBox1.Image = handler.InputImage.Bitmap;
                }
                else
                {
                    MessageBox.Show("Файл не выбран", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void convertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {                 
                if (handler.InputImage == null)
                {
                    MessageBox.Show("Необходимо загрузить изображение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                handler.PartCount = Int32.Parse(textBox1.Text);
                if (handler.PartCount <= 0)
                {
                    MessageBox.Show("Количество частей должно быть не меньше одной", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }                
                handler.PartWidth = handler.InputImage.Width / handler.PartCount;                
                handler.CutImage();//нарезка изображения
                handler.ColorToGray();//перевод изображений в черно-белый вариант                
                await handler.Sobel();//применение оператора собеля                
                pictureBox2.Image = handler.GluingImage();//склейка итогового изображения
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void saveResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureBox2.Image == null)
                {
                    MessageBox.Show("Итоговое изображение отсутствует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                saveFileDialog1.Filter = "PNG Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                saveFileDialog1.Title = "Save an Image File";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs =
                        (System.IO.FileStream)saveFileDialog1.OpenFile();
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            pictureBox2.Image.Save(fs, ImageFormat.Png);
                            break;
                        case 2:
                            pictureBox2.Image.Save(fs, ImageFormat.Jpeg);
                            break;
                        case 3:
                            pictureBox2.Image.Save(fs, ImageFormat.Bmp);
                            break;
                        case 4:
                            pictureBox2.Image.Save(fs, ImageFormat.Gif);
                            break;
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
