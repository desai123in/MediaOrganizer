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
     /****IMP about UI synchronization and calling business/datalayer methods (here by refered to as OM, outside method) asynchronously
        -you can pass ui collection to OM as long as OM doesn't try to update it
        
     ***/
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
                    findFilesToCopyCommand = new DelegateCommand(GetMediaToCopyAsync, CanExecute_GetMedia);
                return findFilesToCopyCommand;
            }
        }

       // public ReactiveCommand<ScalarResult<int>> CopyAllCommand { get; set; }
        public DelegateCommand CopyAllCommand
        {
            get
            {
                if (copyAllCommand == null)
                    copyAllCommand = new DelegateCommand(CopyMediaAsync, CanExecute_CopyMedia);
                return copyAllCommand;
            }
        }

        public bool IsCopyExecuting
        {
            get { return isCopyExecuting; }
            set { isCopyExecuting = value; Notify("IsCopyExecuting"); }
        }

        public bool IsGetExecuting
        {
            get { return isGetExecuting; }
            set { isGetExecuting = value; Notify("isGetExecuting"); }
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
        private bool isGetExecuting = false;

        public Action<object> BrowseAction;

        

        public OrganizePhotoVM()
        {
            //get user settings, previously used value in folder paths

            SearchFolder = settingsManager.GetSettingValue(Screen.OrganizePhotoScreenName, "SearchFolder");
            FromFolder = settingsManager.GetSettingValue(Screen.OrganizePhotoScreenName, "FromFolder");
            ToFolder = settingsManager.GetSettingValue(Screen.OrganizePhotoScreenName, "ToFolder");

            filesToCopy = new ObservableCollection<string>();
            //filesToCopy.Add("test1");
            //filesToCopy.Add("test2");
            //CopyAllCommand = ReactiveCommand.CreateAsyncTask()
            
        }

        private bool CanExecute_CopyMedia(object param)
        {
            return (FilesToCopy.Count > 0 && !IsCopyExecuting);
        }

        private bool CanExecute_GetMedia(object param)
        {
            return (!IsCopyExecuting && !IsGetExecuting);
        }


        async private void CopyMediaAsync(object param)
        {
            if(FilesToCopy.Count > 0)
            {
                IsCopyExecuting = true;
               
                var res = await CopyMediaAsync(FilesToCopy.ToList<string>(), ToFolder);
                FilesToCopy.Clear();
                //back on UI Thread
                IsCopyExecuting = false;

                //below needed to update button enable/disable otherwise its not enabling sometime without clicking on ui
                CommandManager.InvalidateRequerySuggested();
            }
            //var res = await CopyMediaAsync
        }

        async private void GetMediaToCopyAsync(object param)
        {
            IsGetExecuting = true;
            FilesToCopy.Clear();
            var res = await GetMediaToCopyAsync(FromFolder, ToFolder);

            if (res.Errors != null && res.Errors.Count > 0)
            {

            }
            else
            {
                FilesToCopy = new ObservableCollection<string>(res.ResultCollection);
            }
            IsGetExecuting = false;
            //below needed to update button enable/disable otherwise its not enabling sometime without clicking on ui
            CommandManager.InvalidateRequerySuggested();

        }

        async private Task<ScalarResult<int>> CopyMediaAsync(IList<string> fromFiles, string toFolder)
        {
            try
            {
                IMediaOrganizer mediaOrganizer = new PhotoOrganizer();
                
                //ScalarResult<int> res = await Task.Run<ScalarResult<int>>(() => { Task.Delay(8000).Wait(); return new ScalarResult<int>(); });
                //ScalarResult<int> res = await TestWait();
                
                //ScalarResult<int> res = await Task.FromResult<ScalarResult<int>>(mediaOrganizer.CopyMedia(temp,"test"));
                //for some reason Task.FromResult runs on UI Thread
                //also in this method after we get res we don't want to update UI in this method so we should do ConfigureAwait false
                //till this statement we are on UI Thread, entire CopyMedia method will be executed in worker thread
                ScalarResult<int> res = await Task.Run<ScalarResult<int>>(() => mediaOrganizer.CopyMedia(fromFiles, toFolder)).ConfigureAwait(false);
                //still on worker thread as ConfigureAwait is false
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

        //NOT USED
        //FOR TEST ONLY
        async private Task<ScalarResult<int>> TestWait()
        {
            await Task.Delay(8000); 
            return new ScalarResult<int>(); 
        }

        
        async private Task<ListResult<string>> GetMediaToCopyAsync(string from,string to)
        {
            try
            {
                IMediaOrganizer mediaOrganizer = new PhotoOrganizer();
                mediaOrganizer.SearchFolder = searchFolder;
                ListResult<string> res = await Task.Run<ListResult<string>>(() => mediaOrganizer.GetListOfNewMediaMissingInToFolder(from, to)).ConfigureAwait(false);
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
