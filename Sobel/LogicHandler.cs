using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
namespace Sobel
{
    public class LogicHandler
    {
        public List<Bitmap> Images { get; set; }
        public Image<Bgr, byte> InputImage { get; set; }
        public string FilePath { get; set; }
        public int PartWidth { get; set; }
        public List<Image<Gray, byte>> Grays { get; set; }
        public List<Image<Gray, float>> Sobels { get; set; }
        public Bitmap Result { get; set; }
        public int PartCount { get; set; }
        public LogicHandler()
        {
        }
        public void CutImage()//нарезка изображения
        {
            Images = new List<Bitmap>();
            for (int i = 0; i < PartCount - 1; i++)
            {
                Images.Add(new Bitmap(PartWidth, InputImage.Height));
                var graphics = Graphics.FromImage(Images[i]);
                graphics.DrawImage(InputImage.ToBitmap(), new Rectangle(0, 0, PartWidth, InputImage.Height),
                    new Rectangle(PartWidth * i, 0, PartWidth, InputImage.Height), GraphicsUnit.Pixel);
                graphics.Dispose();
            }
            //последняя часть с остатком
            Images.Add(new Bitmap(PartWidth + (InputImage.Width % PartCount), InputImage.Height));
            var graphic = Graphics.FromImage(Images[PartCount - 1]);
            graphic.DrawImage(InputImage.ToBitmap(), new Rectangle(0, 0, PartWidth + (InputImage.Width % PartCount), InputImage.Height),
                new Rectangle(PartWidth * (PartCount - 1), 0, PartWidth + (InputImage.Width % PartCount), InputImage.Height), GraphicsUnit.Pixel);
            graphic.Dispose();
        }
        public void ColorToGray()//перевод изображений в черно-белый вариант
        {        
            Grays = new List<Image<Gray, byte>>();
            for (int i = 0; i < PartCount; i++)
            {
                Grays.Add(new Image<Gray, byte>(Images[i]));

            }
        }
        public async Task Sobel()//применение оператора собеля
        {
            Sobels = new List<Image<Gray, float>>();
            for (int i = 0; i < PartCount; i++)
            {
                await Task.Run(() =>
                {
                    Sobels.Add(Grays[i].Sobel(0, 1, 7).Add(Grays[i].Sobel(1, 0, 7)).AbsDiff(new Gray(0.0)));
                });
            }
        }
        public Bitmap GluingImage()
        {
            Result = new Bitmap(FilePath);
            for (int i = 0; i < PartCount - 1; i++)
            {
                var graphics = Graphics.FromImage(Result);
                graphics.DrawImage(Sobels[i].ToBitmap(), new Rectangle(PartWidth * i, 0, PartWidth, InputImage.Height),
                    new Rectangle(0, 0, PartWidth, InputImage.Height), GraphicsUnit.Pixel);
                graphics.Dispose();
            }
            //последняя часть с остатком
            var graphic = Graphics.FromImage(Result);
            graphic.DrawImage(Sobels[PartCount - 1].ToBitmap(), new Rectangle(PartWidth * (PartCount - 1), 0, PartWidth + (InputImage.Width % PartCount), InputImage.Height),
                new Rectangle(0, 0, PartWidth + (InputImage.Width % PartCount), InputImage.Height), GraphicsUnit.Pixel);
            graphic.Dispose();
            return Result;
        }
    }
}
