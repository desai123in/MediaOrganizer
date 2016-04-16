using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OrganizeMedia;
using OrganizeMedia.Photo;

namespace OrganizeMediaGUI.ViewModel
{
    public class OrganizePhotoVM:BaseViewModel
    {
               
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
              
        public List<string> FilesToCopy
        {
            get { return filesToCopy; }
            set { filesToCopy = value; }
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
                    findFilesToCopyCommand = new DelegateCommand(GetMediaToCopy);
                return findFilesToCopyCommand;
            }
        }

        private string searchFolder = string.Empty;
        private string fromFolder = "FromFolderTest";
        private string toFolder = "ToFolderTest";
        private List<string> filesToCopy;
        private DelegateCommand browseCommand;
        private DelegateCommand findFilesToCopyCommand;
        public Action<object> BrowseAction;
        

        public OrganizePhotoVM()
        {
            //browseCommand = new DelegateCommand(BrowseAction);
        }



        async private void GetMediaToCopy(object param)
        {
            
            IMediaOrganizer mediaOrganizer = new PhotoOrganizer();
            ListResult<string> res = await Task.FromResult<ListResult<string>>(mediaOrganizer.GetListOfNewMediaMissingInToFolder(fromFolder, toFolder));

            FilesToCopy = res.ResultCollection.ToList<string>();

        }
        
        //private async Task<ListResult<string>> GetListOfNewMediaMissingInToFolder(string fromFolder,string toFolder)
        //{
        //    try
        //    {
        //        IMediaOrganizer mediaOrganizer = new PhotoOrganizer();

        //        ListResult<string> res = await Task.FromResult<ListResult<string>>(mediaOrganizer.GetListOfNewMediaMissingInToFolder(fromFolder, toFolder));   
                


        //    }

        //}


    }


   
}
