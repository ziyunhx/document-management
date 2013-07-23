namespace Commons
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Web;
    using System.Web.UI.HtmlControls;

    public class UploadFile
    {
        private HttpPostedFile FileUp;

        private UploadFile()
        {
            this.FileUp = null;
        }

        public UploadFile(HttpPostedFile File)
        {
            this.FileUp = File;
        }

        public static void AddShuiYinPic(string Path, string Path_syp, string Path_sypf)
        {
            Image image = Image.FromFile(Path);
            Image image2 = Image.FromFile(Path_sypf);
            Graphics graphics = Graphics.FromImage(image);
            graphics.DrawImage(image2, new Rectangle(100, 100, image2.Width, image2.Height), 0, 0, image2.Width, image2.Height, GraphicsUnit.Pixel);
            graphics.Dispose();
            image.Save(Path_syp);
            image.Dispose();
        }

        public static void AddShuiYinWord(string Path, string Path_sy, string Words)
        {
            string s = Words;
            Image image = Image.FromFile(Path);
            Graphics graphics = Graphics.FromImage(image);
            graphics.DrawImage(image, 0, 0, image.Width, image.Height);
            Font font = new Font("Verdana", 16f);
            Brush brush = new SolidBrush(Color.White);
            graphics.DrawString(s, font, brush, (float) 100f, (float) 100f);
            graphics.Dispose();
            image.Save(Path_sy);
            image.Dispose();
        }

        public string GetUpFileAllName()
        {
            if (this.FileUp == null)
            {
                return "";
            }
            return this.FileUp.FileName;
        }

        public long GetUpFileContentLength()
        {
            if (this.FileUp == null)
            {
                return 0L;
            }
            return (long) this.FileUp.ContentLength;
        }

        public string GetUpFileExtName()
        {
            if (this.FileUp == null)
            {
                return "";
            }
            return FileUtil.GetFileExtName(this.FileUp.FileName);
        }

        public string GetUpFileName()
        {
            if (this.FileUp == null)
            {
                return "";
            }
            return FileUtil.GetFileName(this.FileUp.FileName);
        }

        public static void MakeNewImage(string originalImagePath, string thumbnailPath)
        {
            int width = 300;
            int height = 300;
            try
            {
                Image image = Image.FromFile(originalImagePath);
                if ((image.Width < width) && (image.Height < height))
                {
                    Size size = new Size(image.Width, image.Height);
                    Image image2 = new Bitmap(size.Width, size.Height);
                    Graphics graphics = Graphics.FromImage(image2);
                    graphics.InterpolationMode = InterpolationMode.High;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.Clear(Color.Transparent);
                    graphics.DrawImage(image, new Rectangle(0, 0, image2.Width, image2.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                    File.Delete(originalImagePath);
                    image2.Save(thumbnailPath, ImageFormat.Jpeg);
                    graphics.Dispose();
                    image.Dispose();
                    image2.Dispose();
                }
                else if (image.Width >= image.Height)
                {
                    Size size2 = new Size(width, height);
                    Image image3 = new Bitmap(size2.Width, size2.Height);
                    Graphics graphics2 = Graphics.FromImage(image3);
                    graphics2.InterpolationMode = InterpolationMode.High;
                    graphics2.SmoothingMode = SmoothingMode.HighQuality;
                    graphics2.Clear(Color.Transparent);
                    graphics2.DrawImage(image, new Rectangle(0, 0, image3.Width, (image3.Height * image.Height) / image.Width), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                    File.Delete(originalImagePath);
                    image3.Save(thumbnailPath, ImageFormat.Jpeg);
                    graphics2.Dispose();
                    image.Dispose();
                    image3.Dispose();
                }
                else
                {
                    Size size3 = new Size(width, height);
                    Image image4 = new Bitmap(size3.Width, size3.Height);
                    Graphics graphics3 = Graphics.FromImage(image4);
                    graphics3.InterpolationMode = InterpolationMode.High;
                    graphics3.SmoothingMode = SmoothingMode.HighQuality;
                    graphics3.Clear(Color.Transparent);
                    graphics3.DrawImage(image, new Rectangle(0, 0, (image4.Width * image.Width) / image.Height, image4.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                    File.Delete(originalImagePath);
                    image4.Save(thumbnailPath, ImageFormat.Jpeg);
                    graphics3.Dispose();
                    image.Dispose();
                    image4.Dispose();
                }
            }
            catch
            {
            }
        }

        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            string str;
            Image image = Image.FromFile(originalImagePath);
            int num = width;
            int num2 = height;
            int x = 0;
            int y = 0;
            int num5 = image.Width;
            int num6 = image.Height;
            if (((str = mode) != null) && (str != "HW"))
            {
                if (!(str == "W"))
                {
                    if (str == "H")
                    {
                        num = (image.Width * height) / image.Height;
                    }
                    else if (str == "Cut")
                    {
                        if ((((double) image.Width) / ((double) image.Height)) > (((double) num) / ((double) num2)))
                        {
                            num6 = image.Height;
                            num5 = (image.Height * num) / num2;
                            y = 0;
                            x = (image.Width - num5) / 2;
                        }
                        else
                        {
                            num5 = image.Width;
                            num6 = (image.Width * height) / num;
                            x = 0;
                            y = (image.Height - num6) / 2;
                        }
                    }
                }
                else
                {
                    num2 = (image.Height * width) / image.Width;
                }
            }
            Image image2 = new Bitmap(num, num2);
            Graphics graphics = Graphics.FromImage(image2);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Color.Transparent);
            graphics.DrawImage(image, new Rectangle(0, 0, num, num2), new Rectangle(x, y, num5, num6), GraphicsUnit.Pixel);
            try
            {
                image2.Save(thumbnailPath, ImageFormat.Jpeg);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                image.Dispose();
                image2.Dispose();
                graphics.Dispose();
            }
        }

        public int UpFileSaveAs(int iSwitch, string strFileAllName)
        {
            if (strFileAllName == "")
            {
                return -2;
            }
            if (this.GetUpFileContentLength() > 0x3d0900L)
            {
                return -3;
            }
            strFileAllName = strFileAllName.Trim();
            if (FileUtil.DictoryCreate(FileUtil.GetDealedFilePath(FileUtil.GetFileDictory(strFileAllName))) != 1)
            {
                return -4;
            }
            bool flag = FileUtil.FileExists(strFileAllName);
            if ((iSwitch != 1) && flag)
            {
                return -1;
            }
            try
            {
                this.FileUp.SaveAs(strFileAllName);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public static string uploadFile(HtmlInputFile File1, string savePath, bool isThumbnail, string delPath, bool isShuiyin, bool isName)
        {
            try
            {
                if (File1.PostedFile.FileName.Trim() != "")
                {
                    string str = File1.PostedFile.ContentType.ToLower();
                    if ((!(str == "image/bmp") && !(str == "image/gif")) && ((!(str == "image/pjpeg") && !(str == "image/png")) && !(str == "image/jpg")))
                    {
                        return "2";
                    }
                    FileInfo info = new FileInfo(File1.PostedFile.FileName);
                    string name = "";
                    if (isName)
                    {
                        name = "eg.jpg";
                    }
                    else
                    {
                        name = info.Name;
                    }
                    string str3 = "s_OBAI" + name;
                    string str4 = "s_" + name;
                    string str5 = "OBAI" + name;
                    string path = HttpContext.Current.Server.MapPath(savePath + name);
                    string thumbnailPath = HttpContext.Current.Server.MapPath(savePath + str3);
                    string str8 = HttpContext.Current.Server.MapPath(savePath + str4);
                    string filename = HttpContext.Current.Server.MapPath(savePath + str5);
                    if (File.Exists(path) && !isName)
                    {
                        return "1";
                    }
                    if ((delPath != "") && File.Exists(HttpContext.Current.Server.MapPath(savePath + delPath)))
                    {
                        File.Delete(HttpContext.Current.Server.MapPath(savePath + delPath));
                    }
                    if (isShuiyin)
                    {
                        File1.PostedFile.SaveAs(filename);
                        File1.PostedFile.SaveAs(path);
                    }
                    else
                    {
                        File1.PostedFile.SaveAs(path);
                    }
                    if (isShuiyin)
                    {
                        AddShuiYinPic(path, filename, HttpContext.Current.Server.MapPath(savePath + "logo.gif"));
                        File.Delete(path);
                    }
                    if (isThumbnail)
                    {
                        if (isShuiyin)
                        {
                            MakeThumbnail(filename, thumbnailPath, 130, 100, "HW");
                        }
                        else
                        {
                            MakeThumbnail(path, str8, 130, 100, "CUT");
                        }
                        if ((delPath != "") && File.Exists(HttpContext.Current.Server.MapPath(savePath + "s_" + delPath)))
                        {
                            File.Delete(HttpContext.Current.Server.MapPath(savePath + "s_" + delPath));
                        }
                    }
                    if (isShuiyin)
                    {
                        return str5;
                    }
                    return name;
                }
                return "3";
            }
            catch
            {
                return "0";
            }
        }
    }
}

