using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace OrganizeMediaGUI.ViewModel
{

    public class BaseViewModel:INotifyPropertyChanged
    {
        static BaseViewModel()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises a PropertyChanged event
        /// </summary>
        protected void Notify(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }

    public class DelegateCommand : ICommand
    {
        Func<object, bool> canExecute;
        Action<object> executeAction;

        public DelegateCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
        }

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
           
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            bool result = true;
            Func<object, bool> canExecuteHandler = this.canExecute;
            if (canExecuteHandler != null)
            {
                result = canExecuteHandler(parameter);
            }

            return result;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                //_internalCanExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                //_internalCanExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        //public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            //CanExecuteChanged(this, new EventArgs());
            //EventHandler handler = this.CanExecuteChanged;
            //if (handler != null)
            //{
            //    handler(this, new EventArgs());
            //}
        }

        public void Execute(object parameter)
        {
            if(executeAction != null)
                this.executeAction(parameter);
        }
    }

    public class FileNameToFilePropertiesConverter : IValueConverter
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {

                var file = new FileInfo((string)value);
                var fileModifedDateTime = file.LastWriteTime.ToString("MM/dd/yy", CultureInfo.InvariantCulture);

                var fileSize = String.Format("{0:N0}KB", (file.Length / 1024f));
                
                var key = string.Format("{0}_[{1}]", fileModifedDateTime, fileSize);

                return key;
            }
            catch(Exception e)
            {
                Log.Error(e);
                return value;
            }
           
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
