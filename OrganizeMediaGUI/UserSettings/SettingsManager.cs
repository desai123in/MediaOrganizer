using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OrganizeMediaGUI.UserSettings
{
    public class SettingsManager:IDisposable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Settings initializedSettings;
        private const string settingsFileName = "MediaOrganizerSettings.xml";
        private Settings currentSettingCached = null;

        public Settings InitializedSettings
        {
            get { return initializedSettings; }
        }

        public SettingsManager()
        {
            Initialize();
        }

        public bool SetSettingValue(string screenName, string settingName,string settingValue)
        {
            bool retVal = true;
            try
            {
                CacheSettingsObject();

                var screen = currentSettingCached.Screens.Where(scr => scr.Name.Equals(screenName)).FirstOrDefault();
                var setting = screen.SettingsForScreen.Where(s => s.Name.Equals(settingName)).FirstOrDefault();

                setting.Value = settingValue;

                return retVal;
            }
            catch (Exception e)
            {
                Log.WarnFormat("Error setting setting value for {0}", settingName);
                Log.Error(e);
                return false;
            }
        }

        public string GetSettingValue(string screenName, string settingName)
        {
            try
            {
                CacheSettingsObject();

                var screen = currentSettingCached.Screens.Where(scr => scr.Name.Equals(screenName)).FirstOrDefault();
                var settingValue = screen.SettingsForScreen.Where(s => s.Name.Equals(settingName)).FirstOrDefault().Value;

                return settingValue;

            }
            catch (Exception e)
            {

                Log.WarnFormat("Error getting setting value for {0}", settingName);
                Log.Error(e);
                return string.Empty;
            }


        }

        private void CacheSettingsObject()
        {
            if (currentSettingCached == null)
            {
                GetCurrentSettings();
            }
        }

        public Settings GetCurrentSettings()
        {
            try
            {

                if (currentSettingCached == null)
                {
                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), settingsFileName);
                    XmlSerializer ser = new XmlSerializer(typeof(Settings));

                    using (XmlReader reader = XmlReader.Create(filePath))
                    {
                        currentSettingCached = (Settings)ser.Deserialize(reader);
                    }
                }
                return currentSettingCached;
            }
            catch (Exception e)
            {
                Log.Warn("Error getting settings from settings file");
                Log.Error(e);
                return null;
            }

        }

        public bool SaveSettings(Settings settings)
        {
            bool retVal = true;
            try
            {

                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), settingsFileName);

                Log.DebugFormat("saving settings file to: {0}", filePath);

                XmlSerializer serializer = new XmlSerializer(settings.GetType());
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    serializer.Serialize(writer, settings);
                }
                return retVal;
            }
            catch (Exception e)
            {
                Log.Warn("Error saving setting file");
                Log.Error(e);
                return false;
            }


        }

        private void Initialize()
        {
            initializedSettings = new Settings();

            var screen = new Screen() { Name = Screen.OrganizePhotoScreenName };
            var setting = new Setting() { Name = "SearchFolder", Value = string.Empty };
            screen.SettingsForScreen.Add(setting);
            setting = new Setting() { Name = "FromFolder", Value = string.Empty };
            screen.SettingsForScreen.Add(setting);
            setting = new Setting() { Name = "ToFolder", Value = string.Empty };
            screen.SettingsForScreen.Add(setting);

            initializedSettings.Screens = new List<Screen>();
            initializedSettings.Screens.Add(screen);

        }

        ~SettingsManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            Log.InfoFormat("Saving setting on Dispose");
            //if its null means it was never invoked and nothing to save
            if(currentSettingCached != null)
            {
                SaveSettings(currentSettingCached);
            }
        }
    }
}
