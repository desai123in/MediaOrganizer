using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizeMediaGUI.UserSettings
{
    public class Screen
    {
        public static readonly string OrganizePhotoScreenName = "OrganizePhotos";

        public string Name { get; set; }
        public string Value { get; set; }

        private List<Setting> settingsForScreen = new List<Setting>();

        public List<Setting> SettingsForScreen
        {
            get { return settingsForScreen; }
            set { settingsForScreen = value; }
        }

    }
}
