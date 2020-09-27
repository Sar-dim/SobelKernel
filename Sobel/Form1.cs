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
        private Image<Bgr, byte> inputImage = null;
        string filePath;
        public Form1()
        {
            InitializeComponent();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    filePath = openFileDialog1.FileName;
                    inputImage = new Image<Bgr, byte>(openFileDialog1.FileName);
                    pictureBox1.Image = inputImage.Bitmap;
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
                if (inputImage == null)
                {
                    MessageBox.Show("Необходимо загрузить изображение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int partCount = Int32.Parse(textBox1.Text);
                if (partCount <= 0)
                {
                    MessageBox.Show("Количество частей должно быть не меньше одной", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                List<Bitmap> images = new List<Bitmap>();                
                int partWidth = inputImage.Width / partCount;
                //нарезка изображения
                for (int i = 0; i < partCount - 1; i++)
                {
                    images.Add(new Bitmap(partWidth, inputImage.Height));                    
                    var graphics = Graphics.FromImage(images[i]);
                    graphics.DrawImage(inputImage.ToBitmap(), new Rectangle(0, 0, partWidth, inputImage.Height), 
                        new Rectangle(partWidth * i, 0, partWidth, inputImage.Height), GraphicsUnit.Pixel);
                    graphics.Dispose();
                }
                //последняя часть с остатком
                images.Add(new Bitmap(partWidth + (inputImage.Width % partCount), inputImage.Height));
                var graphic = Graphics.FromImage(images[partCount - 1]);
                graphic.DrawImage(inputImage.ToBitmap(), new Rectangle(0, 0, partWidth + (inputImage.Width % partCount), inputImage.Height),
                    new Rectangle(partWidth * (partCount - 1), 0, partWidth + (inputImage.Width % partCount), inputImage.Height), GraphicsUnit.Pixel);
                graphic.Dispose();
                //перевод изображений в черно-белый вариант
                List<Image<Gray, byte>> grays = new List<Image<Gray, byte>>();
                for (int i = 0; i < partCount; i++)
                {
                    grays.Add(new Image<Gray, byte>(images[i]));
                    
                }
                //применение оператора собеля
                List<Image<Gray, float>> sobels = new List<Image<Gray, float>>();
                for (int i = 0; i < partCount; i++)
                {
                    await Task.Run(() =>
                    {
                        sobels.Add(grays[i].Sobel(0, 1, 7).Add(grays[i].Sobel(1, 0, 7)).AbsDiff(new Gray(0.0)));
                    });                 
                }
                //склейка итогового изображения
                Bitmap result = new Bitmap(filePath);
                for (int i = 0; i < partCount - 1; i++)
                {
                    var graphics = Graphics.FromImage(result);
                    graphics.DrawImage(sobels[i].ToBitmap(), new Rectangle(partWidth * i, 0, partWidth, inputImage.Height),
                        new Rectangle(0, 0, partWidth, inputImage.Height), GraphicsUnit.Pixel);
                    graphics.Dispose();
                }
                //последняя часть с остатком
                graphic = Graphics.FromImage(result);
                graphic.DrawImage(sobels[partCount - 1].ToBitmap(), new Rectangle(partWidth * (partCount - 1), 0, partWidth + (inputImage.Width % partCount), inputImage.Height),
                    new Rectangle(0, 0, partWidth + (inputImage.Width % partCount), inputImage.Height), GraphicsUnit.Pixel);
                graphic.Dispose();
                pictureBox2.Image = result;
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
