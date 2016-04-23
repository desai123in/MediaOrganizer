using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace OrganizeMedia.Photo
{
    public class PhotoOrganizer : IMediaOrganizer
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string ModifedDateTimeFormat = "yyyyMMdd,HH:mm:ss.ff";
        private List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

        //value[0] would be kept filepath of dups
        private Dictionary<string, List<string>> duplicates = new Dictionary<string, List<string>>();
        private string searchFolder = string.Empty;
        private const string tempPathFolderAppsettingKey = "debugtemppath";
        #region IMediaOrganizer members

        public string SearchFolder
        {
            get
            {
                return searchFolder;
            }
            set
            {
                searchFolder = value;
            }
        }

        /// <summary>
        /// Logic: create dictionary of all photos in toFolder with key as (moddt+len)
        ///  go through each fils in fromFolder and check if exist in toFolder based on key, add to list of file to copy
        ///  
        /// </summary>
        /// <param name="fromFolder"></param>
        /// <param name="toFolder"></param>
        /// <returns></returns>
        public ListResult<string> GetListOfNewMediaMissingInToFolder(string fromFolder, string toFolder)
        {
            var result = new ListResult<string>();
            result.Errors = new List<string>();
            result.Logs = new List<string>();

            Task.Delay(5000).Wait();

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


                var issueFile = uniqToFiles.Values.Where(v => v.Contains("DSC_8091")).FirstOrDefault();
                var issuekey = uniqToFiles.FirstOrDefault(kv => kv.Value.Equals(issueFile)).Key;

                //for debugging
                var uniqToFilesKeyValue = new List<string>();
                foreach (var keyvalue in uniqToFiles)
                {
                    uniqToFilesKeyValue.Add(string.Format("{0}#{1}", keyvalue.Key, keyvalue.Value));
                }

                int totalNumOfDups = allToFilesInfo.Count() - uniqToFiles.Count;



                Log.InfoFormat("{0} dup image files found in fromFolder: {1}", totalNumOfDups, fromFolder);

                //now we have all uniq files in toFolder in uniqToFiles dict, and all dups with all their occurences in dupFiles dict.
                allFromFilesInfo = GetFilesInfoFromFolder(fromFolder);
                Log.InfoFormat("{0} image files found in fromFolder: {1}", allFromFilesInfo.Count(), fromFolder);

                //go through all fromFolder files, and if doesn't exist in uniqToFiles dict then copy 
                int alreadyExistCount = 0;
                //for debugging
                var filesToMoveKeyValue = new List<string>();
                foreach (var file in allFromFilesInfo)
                {
                    var path = file.FullName;
                    var key = GetFileKey(path);

                    var existingToPath = string.Empty;
                    if (!uniqToFiles.TryGetValue(key, out existingToPath))
                    {
                        filesToMove.Add(path);
                        //for debugging
                        filesToMoveKeyValue.Add(string.Format("{0}#{1}", key, path));
                    }
                    else
                    {
                        alreadyExistCount++;
                    }
                }
                Log.InfoFormat("{0} new image files found in fromFolder: {1} which does not exist in toFolder: {0}", filesToMove.Count, fromFolder, toFolder);


                //following code is for verification

                try
                {
                    CreateDebugFiles(uniqToFilesKeyValue, "uniqInToFolder.txt");
                    CreateDebugFiles(filesToMoveKeyValue, "toMove.txt");

                    var issueFiles = uniqToFilesKeyValue.Where(u => filesToMoveKeyValue.Contains(GetFileNameFromkeyvalue(u))).Select(u => u);
                    if (issueFiles.Count() > 0)
                    {
                        foreach (var val in issueFiles)
                            Log.WarnFormat("Some issue in file matching:{0}", val);
                    }
                    else
                    {
                        Log.Info("No Issues in file matching based on filename");
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Error performing verfication");
                    Log.Error(e);
                }

            }

            catch (Exception e)
            {
                Log.Error(e);
                result.Errors.Add(string.Format("Unable to get new files to copy, Error: {0}", e.Message));
                return result;
            }


            return result;
        }

        private string GetFileNameFromkeyvalue(string input)
        {
            try
            {
                var filepath = input.Split('#')[1];
                var filename = Path.GetFileName(filepath);
                return filename;
            }
            catch (Exception e)
            {
                Log.Warn("Error splitting filename from keyvalue");
                Log.Error(e);
                return string.Empty;

            }


        }

        private void CreateDebugFiles(List<string> keyValueList, string fileName)
        {
            try
            {
                var tempPath = ConfigurationSettings.AppSettings[tempPathFolderAppsettingKey];

                var filePath = Path.Combine(tempPath, fileName);
                File.WriteAllLines(filePath, keyValueList);
            }
            catch (Exception e)
            {
                Log.WarnFormat("error creating debug files");
                Log.Error(e);
            }


        }


        public ListResult<string> GetDups(string folder, IList<string> ignoreFolders)
        {
            throw new NotImplementedException();
        }

        public ScalarResult<int> CopyMedia(IList<string> fromFiles, string toFolder)
        {
            ScalarResult<int> result = new ScalarResult<int>();
            result.Logs = new List<string>();
            result.Errors = new List<string>();

            
            int numFilesCopied = 0;

            foreach (string filePath in fromFiles)
            {
                var fileName = Path.GetFileName(filePath);
                try
                {
                    File.Copy(filePath, Path.Combine(toFolder,fileName), true);
                    numFilesCopied++;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    result.AddErrorFormat("Could not copy file: {0}", filePath);
                }
            }

            result.ResultValue = numFilesCopied;
            result.AddLogFormat("{0} out of {1} files copied succesfully.", numFilesCopied, fromFiles.Count);

            return result;

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
