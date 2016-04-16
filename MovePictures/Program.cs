using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using log4net;

namespace OrganizeMedia
{
    class Program
    {
        static Program()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            string from = @"C:\Users\Dharmesh\Pictures\2015\Iphone\DCIM";
            string to = @"C:\Users\Dharmesh\Pictures\2015\Iphone\IPhonePictures\";
            string logFile = @"C:\temp\MovePictures.log";
            string toVideo = @"C:\Users\Dharmesh\Pictures\2015\Iphone\IphoneVideos\";

            int iphonePicCount = 0;
            int noniphonePicCount =0;
            int videoFilesCount = 0;
            foreach (var dir in Directory.GetDirectories(from))
            {
                foreach (var path in Directory.GetFiles(dir))
                {

                    if (ImageExtensions.Contains(Path.GetExtension(path).ToUpperInvariant()))
                    {
                        try
                        {
                            bool isIphonePic = false;
                            using (Image image = new Bitmap(path))
                            {
                                var propItems = image.PropertyItems;

                                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                                string manufacturer = encoding.GetString(propItems[1].Value);

                                isIphonePic = manufacturer.Contains("iPhone");
                                Console.WriteLine("Picture: {0} IsIphonePic: {1}", path, isIphonePic); // full path 
                                log.InfoFormat("Picture: {0} IsIphonePic: {1}", path, isIphonePic);

                                //File.AppendAllText(logFile, string.Format("Picture: {0} IsIphonePic: {1}", path, isIphonePic) + Environment.NewLine);
                                if (isIphonePic)
                                {                                    
                                    iphonePicCount++;
                                }
                                else
                                {
                                    noniphonePicCount++;
                                }
                            }
                            if (isIphonePic)
                            {
                                File.Move(path, to + Path.GetFileName(path));
                            }

                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Could not open file: {0}", path);
                            log.Error(e);                            
                        }
                    }
                    else
                    {
                        //its probably video file,
                        File.Move(path, toVideo + Path.GetFileName(path));
                        videoFilesCount++;
                    }

                }
            }
            Console.WriteLine("Total Iphone pictures: {0}", iphonePicCount);
            log.InfoFormat("Total Iphone pictures: {0}", iphonePicCount);

            //File.AppendAllText(logFile, string.Format("Total Iphone pictures: {0}", iphonePicCount) + Environment.NewLine);
            Console.WriteLine("Total Non Iphone pictures: {0}", noniphonePicCount);
            log.InfoFormat("Total Non Iphone pictures: {0}", noniphonePicCount);

            Console.WriteLine("Total Video files: {0}", videoFilesCount);
            log.InfoFormat("Total Video files: {0}", videoFilesCount);
            //File.AppendAllText(logFile, string.Format("Total Non Iphone pictures: {0}", noniphonePicCount) + Environment.NewLine);
            Console.Read();
        }
    }
}
