using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public DelegateCommand BrowseCommand
        {
            get {
                if (browseCommand == null)
                    browseCommand = new DelegateCommand(BrowseAction);
                    return browseCommand; 
                }            
        }

        private string searchFolder = string.Empty;
        private string fromFolder = "FromFolderTest";
        private string toFolder = "ToFolderTest";
        private DelegateCommand browseCommand;
        public Action<object> BrowseAction;
        

        public OrganizePhotoVM()
        {
            //browseCommand = new DelegateCommand(BrowseAction);
        }



    }


   
}
