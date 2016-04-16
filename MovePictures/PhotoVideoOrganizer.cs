using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Globalization;
using log4net;

namespace OrganizeMedia
{
    public class PhotoVideoOrganizer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static PhotoVideoOrganizer()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">@"C:\Users\Dharmesh\Pictures\2015\Iphone\DCIM"</param>
        /// <param name="to">@"C:\Users\Dharmesh\Pictures\2015\Iphone\IPhonePictures\"</param>
        /// <param name="videoFrom">@"C:\Users\Dharmesh\Pictures\2015\Iphone\DCIM"</param>
        /// <param name="videoTo"> @"C:\Users\Dharmesh\Pictures\2015\Iphone\IphoneVideos\"</param>
        public PhotoVideoOrganizer(string from, string to, string videoFrom, string videoTo)
        {
            FromFolder = from;
            ToFolder = to;
            MoveVideoFrom = videoFrom;
            MoveVideoTo = videoTo;
        }

        public PhotoVideoOrganizer(string from, string to)
        {
            FromFolder = from;
            ToFolder = to;
            //MoveVideoFrom = videoFrom;
            //MoveVideoTo = videoTo;
        }

        public PhotoVideoOrganizer(string from, string to, string dupSearchFolder)
            : this(from, to)
        {
            SearchFolderForDups = dupSearchFolder;
        }

        private string _fromFolder;
        public string FromFolder
        {
            get
            {
                return _fromFolder;
            }
            set
            {
                if (!checkIfValidFolder(value))
                {
                    throw new Exception(string.Format("Folder:{0} does not exist", value));
                }
                else
                {
                    _fromFolder = value;
                }
            }
        }

        private string _toFolder;
        public string ToFolder
        {
            get
            {
                return _toFolder;
            }
            set
            {
                if (!checkIfValidFolder(value))
                {
                    throw new Exception(string.Format("Folder:{0} does not exist", value));
                }
                else
                {
                    _toFolder = value;
                }
            }
        }
        public string SearchFolderForDups { get; set; }

        public string MoveVideoTo { get; set; }
        public string MoveVideoFrom { get; set; }


        private List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };
        private List<string> VideoExtensions = new List<string> { ".WMV", ".MOV", ".AVI", ".MP4", ".MPEG", ".MPG" };

        private readonly string ModifedDateTimeFormat = "yyyyMMdd,HH:mm:ss.ff";
        private readonly int ManufacturerPropertyIndex = 1;

        private bool checkIfValidFolder(string folderPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            if (!dirInfo.Exists)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //NOT USED
        private void SearchDirectory(string dir, List<string> fileList, Func<string, bool> includeFile)
        {

            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        if (includeFile(f))
                            fileList.Add(f);
                    }
                    SearchDirectory(d, fileList, includeFile);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        //NOT USED
        private bool isDupBasedOnTimeStampFromImageProperties(string image1Path, string image2Path)
        {
            string cameraTime1 = string.Empty;
            string cameraTime2 = string.Empty;

            using (Image image = new Bitmap(image1Path))
            {
                cameraTime1 = GetCameraTimeStamp(image);
            }
            using (Image image = new Bitmap(image2Path))
            {
                cameraTime2 = GetCameraTimeStamp(image);
            }

            var retVal = cameraTime1.Equals(cameraTime2);

            return retVal;

        }
        private bool isDupBasedOnFileSize(string image1Path, string image2Path)
        {
            long fileSize1 = (new FileInfo(image1Path)).Length;
            long fileSize2 = (new FileInfo(image2Path)).Length;

            return fileSize1.Equals(fileSize2);
        }


        public int BackupPictures()
        {
            //get all files from From folder order by Modified timestamp
            //

            Dictionary<string, string> toFiles = new Dictionary<string, string>();
            Dictionary<string, List<string>> duplicates = new Dictionary<string, List<string>>();
            List<string> filesToMove = new List<string>();

            var allImageToFiles = GetImageFilesInfoFromFolder(ToFolder);

            log.InfoFormat("Total image files in ToFolder:{0} = {1}", ToFolder, allImageToFiles.Count());

            foreach (var file in allImageToFiles)
            {
                var filePath = file.FullName;

                var key = GetFileKey(filePath);

                string existingValue;
                if (toFiles.TryGetValue(key, out existingValue))
                {
                    //probably duplicate

                    List<string> dupFiles;
                    if (duplicates.TryGetValue(key, out dupFiles))
                    {
                        dupFiles.Add(filePath);
                    }
                    else
                    {
                        dupFiles = new List<string>();
                        dupFiles.Add(filePath);
                        duplicates.Add(key, dupFiles);
                    }
                }
                else
                {
                    toFiles[key] = filePath;
                }
            }
            Dictionary<string, List<string>> DupsWithKeptValue = new Dictionary<string, List<string>>();

            int totalDups = 0;
            foreach (var keyValuePair in duplicates)
            {
                var kept = toFiles[keyValuePair.Key];
                DupsWithKeptValue.Add(kept, keyValuePair.Value);
                totalDups += keyValuePair.Value.Count;
            }

            log.InfoFormat("Total duplicate image files in ToFolder:{0} = {1}", ToFolder, totalDups);

            
            var allImageFilesFrom = GetImageFilesInfoFromFolder(FromFolder);
            log.InfoFormat("Total image files in FromFolder:{0} = {1}", FromFolder, allImageFilesFrom.Count());

            
            int alreadyExistCount = 0;
            foreach (var file in allImageFilesFrom)
            {
                var path = file.FullName;
                var key = GetFileKey(path);

                var existingToPath = string.Empty;
                if (!toFiles.TryGetValue(key, out existingToPath))
                {                  
                    filesToMove.Add(path);
                }
                else
                {
                    alreadyExistCount++;
                }
            }

            log.InfoFormat("image files already exists in {0} = {1}", ToFolder,alreadyExistCount);
            log.InfoFormat("image files to move from {0} TO {1} = {2}", FromFolder, ToFolder, filesToMove.Count);

            return 0;
        }

        private string GetFileKey(string filePath)
        {
            var file = new FileInfo(filePath);
            var fileModifedDateTime = file.LastWriteTime.ToString(ModifedDateTimeFormat, CultureInfo.InvariantCulture);
            var fileSize = file.Length;

            var key = string.Format("{0}_{1}", fileModifedDateTime, fileSize);

            return key;
        }

        private IEnumerable<FileInfo> GetImageFilesInfoFromFolder(string folder)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            var allFilesInfo = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            var allImageToFiles = allFilesInfo.Where(f => ImageExtensions.Contains(Path.GetExtension(f.Extension).ToUpperInvariant()));
            return allImageToFiles;
        }

        private int MoveVideoFiles()
        {
            log.InfoFormat("Moving Video files From: {0}", MoveVideoFrom);
            log.InfoFormat("Moving Video files To: {0}", MoveVideoTo);
            int videoFileCount = 0;
            foreach (var dir in Directory.GetDirectories(MoveVideoFrom))
            {
                foreach (var path in Directory.GetFiles(dir))
                {
                    //check if its video file
                    if (VideoExtensions.Contains(Path.GetExtension(path).ToUpperInvariant()))
                    {
                        videoFileCount++;
                        try
                        {
                            File.Move(path, MoveVideoTo + Path.GetFileName(path));
                        }
                        catch (Exception e)
                        {
                            log.Error(e);
                        }
                    }
                }
                log.InfoFormat("Total video files moved: {0}", videoFileCount);
            }
            return videoFileCount;
        }

        /// <summary>
        /// This method will move pictures from folder to To folder all pictures taken from specific manufacturer
        /// 
        /// </summary>
        /// <param name="cameraModelManufacturerStringToMatch">its string which will be matched to see if photo is taken from that manufacturer.
        /// sample value: iPhone</param>
        private int MovePicturesTakenFromSpecificCameraModel(string cameraModelManufacturerStringToMatch)
        {

            int givenCameraModelPicCount = 0;
            int picCountNotFromGivenCameraModel = 0;

            foreach (var dir in Directory.GetDirectories(FromFolder))
            {
                foreach (var path in Directory.GetFiles(dir))
                {
                    //check if its image file
                    if (ImageExtensions.Contains(Path.GetExtension(path).ToUpperInvariant()))
                    {
                        try
                        {
                            bool isPiceFromGivenCameraModel = false;
                            using (Image image = new Bitmap(path))
                            {
                                string manufacturer = GetCamerModelManufacturer(image);

                                isPiceFromGivenCameraModel = manufacturer.Contains(cameraModelManufacturerStringToMatch);
                                log.InfoFormat("Picture: {0} isPiceFromGivenCameraModel: {1}", path, isPiceFromGivenCameraModel);

                                if (isPiceFromGivenCameraModel)
                                {
                                    givenCameraModelPicCount++;
                                }
                                else
                                {
                                    picCountNotFromGivenCameraModel++;
                                }
                            }
                            if (isPiceFromGivenCameraModel)
                            {
                                File.Move(path, ToFolder + Path.GetFileName(path));
                            }

                        }
                        catch (Exception e)
                        {
                            log.Error(e);
                        }
                    }
                }
            }
            log.InfoFormat("Total pictures found from Model:{0}: {1}", cameraModelManufacturerStringToMatch, givenCameraModelPicCount);
            log.InfoFormat("Total pictures from NOT from Model:{0}: {1}", cameraModelManufacturerStringToMatch, picCountNotFromGivenCameraModel);

            return givenCameraModelPicCount;

        }

        private string GetCamerModelManufacturer(Image image)
        {
            var propItems = image.PropertyItems;

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            string manufacturer = encoding.GetString(propItems[ManufacturerPropertyIndex].Value);

            foreach (var prop in propItems)
            {
                string stringValue = encoding.GetString(prop.Value);
            }

            return manufacturer;
        }

        private string GetCameraTimeStamp(Image image)
        {

            var propItems = image.PropertyItems;

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            string timestamp = encoding.GetString(propItems[7].Value);

            return timestamp;
        }

        private string GetCameraTimeStamp(string imagePath)
        {
            string cameraTimeStamp = string.Empty;

            using (Image image = new Bitmap(imagePath))
            {
                cameraTimeStamp = GetCameraTimeStamp(image);
            }

            return cameraTimeStamp;
        }

    }
}



