using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TrackPrintScreen
{
    class Screenshotter
    {
        ImageFormat imageFormat;
        Options options;
        public Screenshotter(Options options)
        {
            imageFormat = ImageFormat.Jpeg;
            this.options = options;
        }

        public void Shot(bool special = false)
        {
            DateTime now = DateTime.Now;

            string path = options.FolderPath;
            char lastChar = path[path.Length - 1];
            if(lastChar != '\\' || lastChar != '/')
            {
                path += "\\";
            }
            path += now.ToString("MM dd yy HH-mm-ss");
            if (special) path = path + "  1  ";
            path += ".jpg";



            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;
            
            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap.Size);
                }
                
                ImageCodecInfo myImageCodecInfo;
                Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;


                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                myEncoder = Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                // Save the bitmap as a JPEG file with quality level 75.
                myEncoderParameter = new EncoderParameter(myEncoder, 75L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bitmap.Save(path, myImageCodecInfo, myEncoderParameters);
            }
        }

        public void UpdateOptions(Options options)
        {
            this.options = options;
        }



        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
