namespace Commons
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class WebPageBitmap
    {
        private int Height;
        private WebBrowser MyBrowser;
        private string URL;
        private int Width;

        public WebPageBitmap(string url, int width, int height)
        {
            this.URL = url;
            this.Width = width;
            this.Height = height;
            this.MyBrowser = new WebBrowser();
            this.MyBrowser.ScrollBarsEnabled = false;
            this.MyBrowser.Size = new Size(this.Width, this.Height);
        }

        public Bitmap DrawBitmap(int theight, int twidth)
        {
            Bitmap bitmap3;
            Bitmap bitmap = new Bitmap(this.Width, this.Height);
            Rectangle targetBounds = new Rectangle(0, 0, this.Width, this.Height);
            this.MyBrowser.DrawToBitmap(bitmap, targetBounds);
            Image image = bitmap;
            Bitmap bitmap2 = new Bitmap(twidth, theight, image.PixelFormat);
            Graphics graphics = Graphics.FromImage(bitmap2);
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            Rectangle rect = new Rectangle(0, 0, twidth, theight);
            graphics.DrawImage(image, rect);
            try
            {
                bitmap3 = bitmap2;
            }
            catch
            {
                bitmap3 = null;
            }
            finally
            {
                image.Dispose();
                image = null;
                this.MyBrowser.Dispose();
                this.MyBrowser = null;
            }
            return bitmap3;
        }

        public void GetIt()
        {
            this.MyBrowser.Navigate(this.URL);
            while (this.MyBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }
    }
}

