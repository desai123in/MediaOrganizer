using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizeMediaGUI.UserSettings
{
    public class Settings
    {
        private List<Screen> screens;

        public List<Screen> Screens
        {
            get { return screens; }
            set { screens = value; }
        }
        
    }
}
