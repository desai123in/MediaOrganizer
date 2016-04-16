using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizeMedia.Photo
{
    public class PhotoOrganizer : IMediaOrganizer
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string ModifedDateTimeFormat = "yyyyMMdd,HH:mm:ss.ff";
        private List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

        //value[0] would be kept filepath of dups
        private Dictionary<string, List<string>> duplicates = new Dictionary<string, List<string>>();

        #region IMediaOrganizer members
        /// <summary>
        /// Logic: create dictionary of all photos in toFolder with key as (moddt+len)
        ///  go through each fils in fromFolder and check if exist in toFolder based on key, if not copy
        /// </summary>
        /// <param name="fromFolder"></param>
        /// <param name="toFolder"></param>
        /// <returns></returns>
        public ListResult<string> GetListOfNewMediaMissingInToFolder(string fromFolder, string toFolder)
        {
            var result = new ListResult<string>();
            result.Errors = new List<string>();
            result.Logs = new List<string>();


            IEnumerable<FileInfo> allToFilesInfo = null; 
            Dictionary<string, string> uniqToFiles = new Dictionary<string, string>();

            IEnumerable<FileInfo> allFromFilesInfo = null;
            List<string> filesToMove = new List<string>();

            result.ResultCollection = filesToMove;

            try
            {

                //get file informations about all the photo files in toFolder
                allToFilesInfo = GetFilesInfoFromFolder(toFolder);
                Log.InfoFormat("{0} image files found in toFolder: {1}", allToFilesInfo.Count(), toFolder);


                //value[0] would be kept filepath of dups

                foreach (var fileInfo in allToFilesInfo)
                {
                    var filePath = fileInfo.FullName;
                    var key = GetFileKey(filePath);

                    string existingValue;
                    if (uniqToFiles.TryGetValue(key, out existingValue))
                    {
                        //its duplicate
                        List<string> dupFiles;

                        //if key is already created then add this key's another dup value
                        if (duplicates.TryGetValue(key, out dupFiles))
                        {
                            //this will be 3rd,4th,5th.. value of dup
                            dupFiles.Add(filePath);
                        }
                        else
                        {
                            dupFiles = new List<string>();
                            //0th element is the 1st value of dup we kept in uniqToFiles
                            dupFiles.Add(existingValue);
                            //this will be 2nd value of dup
                            dupFiles.Add(filePath);
                            duplicates.Add(key, dupFiles);
                        }
                    }
                    else
                    {
                        uniqToFiles[key] = filePath;
                    }
                }

                int totalNumOfDups = allToFilesInfo.Count() - uniqToFiles.Count;

                Log.InfoFormat("{0} dup image files found in fromFolder: {1}", totalNumOfDups, fromFolder);

                //now we have all uniq files in toFolder in uniqToFiles dict, and all dups with all their occurences in dupFiles dict.
                allFromFilesInfo = GetFilesInfoFromFolder(fromFolder);
                Log.InfoFormat("{0} image files found in fromFolder: {1}", allFromFilesInfo.Count(), fromFolder);

                //go through all fromFolder files, and if doesn't exist in uniqToFiles dict then copy 
                int alreadyExistCount = 0;
                foreach (var file in allFromFilesInfo)
                {
                    var path = file.FullName;
                    var key = GetFileKey(path);

                    var existingToPath = string.Empty;
                    if (!uniqToFiles.TryGetValue(key, out existingToPath))
                    {
                        filesToMove.Add(path);
                    }
                    else
                    {
                        alreadyExistCount++;
                    }
                }
                Log.InfoFormat("{0} new image files found in fromFolder: {1} which does not exist in toFolder: {0}", filesToMove.Count, fromFolder,toFolder);
            }

            catch (Exception e)
            {
                Log.Error(e);
                result.Errors.Add(string.Format("Unable to get new files to copy, Error: {0}",e.Message));
                return result;
            }
                       

            return result;
        }


        public ListResult<string> GetDups(string folder)
        {
            throw new NotImplementedException();
        }

        public ScalarResult<int> CopyMedia(List<string> fromFiles, string toFolder)
        {
            throw new NotImplementedException();
        }

        #endregion

        private string GetFileKey(string filePath)
        {
            var file = new FileInfo(filePath);
            var fileModifedDateTime = file.LastWriteTime.ToString(ModifedDateTimeFormat, CultureInfo.InvariantCulture);
            var fileSize = file.Length;

            var key = string.Format("{0}_{1}", fileModifedDateTime, fileSize);

            return key;
        }

        private IEnumerable<FileInfo> GetFilesInfoFromFolder(string folder)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            var allFilesInfo = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            var allImageToFiles = allFilesInfo.Where(f => ImageExtensions.Contains(Path.GetExtension(f.Extension).ToUpperInvariant()));
            return allImageToFiles;
        }

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

    }
}
