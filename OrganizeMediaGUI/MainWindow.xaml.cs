using OrganizeMediaGUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
namespace OrganizeMediaGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OrganizePhotoVM vm;
        
        /**
         * http://stackoverflow.com/questions/6841148/add-image-to-stackpanel-in-programmatically
         * 
         * **/
        public MainWindow()
        {
            InitializeComponent();
            vm = new OrganizePhotoVM();
            vm.BrowseAction = BrowseFolder;
            this.DataContext = vm;

        }

        //NOT USED
        private void Button_Click_FromFolder(object sender, RoutedEventArgs e)
        {
            Gat.Controls.OpenDialogView openDialog = new Gat.Controls.OpenDialogView();
            Gat.Controls.OpenDialogViewModel vmopenDialog = (Gat.Controls.OpenDialogViewModel)openDialog.DataContext;
            vmopenDialog.IsDirectoryChooser = true;
            // Adding file filter
            vmopenDialog.AddFileFilterExtension(".txt");

            // Show dialog and take result into account
            bool? result = vmopenDialog.Show();
            if (result == true)
            {
                // Get selected file path
                vm.FromFolder = vmopenDialog.SelectedFilePath;
            }
            else
            {
                vm.FromFolder = string.Empty;
            }
        }

        //All browse buttons execute this passing textbox to update.
        private void BrowseFolder(object o)
        {
            var textBox = o as System.Windows.Controls.TextBox;
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            if(textBox.Name.Contains("Search"))
                openFolderDialog.RootFolder = Environment.SpecialFolder.MyPictures;
            else
                openFolderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            DialogResult result = openFolderDialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {                
                textBox.Text = openFolderDialog.SelectedPath;
                //this will require to update viewmodel property
                BindingOperations.GetBindingExpression(textBox, System.Windows.Controls.TextBox.TextProperty).UpdateSource();                
            }
            
        }
    }
}
