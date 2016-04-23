using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OrganizeMedia;
using OrganizeMedia.Photo;
using OrganizeMediaGUI.UserSettings;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace OrganizeMediaGUI.ViewModel
{
    public class OrganizePhotoVM:BaseViewModel
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
               

        public string SearchFolder
        {
            get { return searchFolder; }
            set { searchFolder = value; Notify("SearchFolder"); }
        }
       
        public string FromFolder
        {
            get { return fromFolder; }
            set { fromFolder = value; Notify("FromFolder"); }
        }
        
        public string ToFolder
        {
            get { return toFolder; }
            set { toFolder = value; Notify("ToFolder"); }
        }
              
        public ObservableCollection<string> FilesToCopy
        {
            get { return filesToCopy; }
            set { filesToCopy = value; Notify("FilesToCopy"); }
        }
        

        public DelegateCommand BrowseCommand
        {
            get {
                if (browseCommand == null)
                    browseCommand = new DelegateCommand(BrowseAction);
                    return browseCommand; 
                }            
        }

        public DelegateCommand FindFilesCopyCommand
        {
            get
            {
                if (findFilesToCopyCommand == null)
                    findFilesToCopyCommand = new DelegateCommand(GetMediaToCopyAsync);
                return findFilesToCopyCommand;
            }
        }

       // public ReactiveCommand<ScalarResult<int>> CopyAllCommand { get; set; }
        public DelegateCommand CopyAllCommand
        {
            get
            {
                if (copyAllCommand == null)
                    copyAllCommand = new DelegateCommand(CopyMediaAsync);
                return copyAllCommand;
            }
        }

        public bool IsCopyExecuting
        {
            get { return isCopyExecuting; }
            set { isCopyExecuting = value; Notify("IsCopyExecuting"); }
        }

        private string searchFolder = string.Empty;
        private string fromFolder = "FromFolderTest";
        private string toFolder = "ToFolderTest";
        private ObservableCollection<string> filesToCopy;
        private DelegateCommand browseCommand;
        private DelegateCommand findFilesToCopyCommand;
        private DelegateCommand copyAllCommand;
        private SettingsManager settingsManager = new SettingsManager();
        private bool isCopyExecuting = false;

        public Action<object> BrowseAction;

        

        public OrganizePhotoVM()
        {
            //get user settings, previously used value in folder paths

            SearchFolder = settingsManager.GetSettingValue(Screen.OrganizePhotoScreenName, "SearchFolder");
            FromFolder = settingsManager.GetSettingValue(Screen.OrganizePhotoScreenName, "FromFolder");
            ToFolder = settingsManager.GetSettingValue(Screen.OrganizePhotoScreenName, "ToFolder");

            //CopyAllCommand = ReactiveCommand.CreateAsyncTask()
            
        }

        async private void CopyMediaAsync(object param)
        {
            if(FilesToCopy.Count > 0)
            {
                IsCopyExecuting = true;
                var res = await CopyMediaAsync(FilesToCopy.ToList<string>(), ToFolder);
                FilesToCopy.Clear();
                IsCopyExecuting = false;
            }
            //var res = await CopyMediaAsync
        }

        async private void GetMediaToCopyAsync(object param)
        {

            var res = await GetMediaToCopyAsync(FromFolder, ToFolder);

            if (res.Errors != null && res.Errors.Count > 0)
            {

            }
            else
            {
                FilesToCopy = new ObservableCollection<string>(res.ResultCollection);
            }

        }

        async private Task<ScalarResult<int>> CopyMediaAsync(IList<string> fromFiles, string toFolder)
        {
            try
            {
                IMediaOrganizer mediaOrganizer = new PhotoOrganizer();

                ScalarResult<int> res = await Task.Run<ScalarResult<int>>(() => { Task.Delay(5000).Wait(); return new ScalarResult<int>(); });//await Task.FromResult<ScalarResult<int>>(mediaOrganizer.CopyMedia(fromFiles, toFolder));
                return res;
                
            }
            catch (Exception e)
            {
                Log.Error(e);
                ScalarResult<int> emptyRes = new ScalarResult<int>();
                emptyRes.Errors.Add("Failed To Copy new media");
                return emptyRes;
            }

        }

        
        async private Task<ListResult<string>> GetMediaToCopyAsync(string from,string to)
        {
            try
            {
                IMediaOrganizer mediaOrganizer = new PhotoOrganizer();
                mediaOrganizer.SearchFolder = searchFolder;
                ListResult<string> res = await Task.Run<ListResult<string>>(() => mediaOrganizer.GetListOfNewMediaMissingInToFolder(from, to));
                    //await Task.Run<ListResult<string>>(() => { Task.Delay(5000).Wait(); return new ListResult<string>(); });

                return res;
            }
            catch(Exception e)
            {
                Log.Error(e);
                ListResult<string> emptyRes = new ListResult<string>();
                emptyRes.Errors.Add("Failed To Get new media");
                return emptyRes;

            }

        }

        
        //private async Task<ListResult<string>> GetListOfNewMediaMissingInToFolder(string fromFolder,string toFolder)
        //{
        //    try
        //    {
        //        IMediaOrganizer mediaOrganizer = new PhotoOrganizer();

        //        ListResult<string> res = await Task.FromResult<ListResult<string>>(mediaOrganizer.GetListOfNewMediaMissingInToFolder(fromFolder, toFolder));   
                


        //    }

        //}



        public void Dispose(object sender, CancelEventArgs e)
        {
            Log.Info("Saving setting from PhotoViewModel");

            //save settings            
            settingsManager.SetSettingValue(Screen.OrganizePhotoScreenName, "SearchFolder", SearchFolder);
            settingsManager.SetSettingValue(Screen.OrganizePhotoScreenName, "FromFolder", FromFolder);
            settingsManager.SetSettingValue(Screen.OrganizePhotoScreenName, "ToFolder", ToFolder); 
        }
    }


   
}
