using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrganizeMedia;
using OrganizeMedia.Photo;
using System.Collections.Generic;
using log4net;
using System.Linq;
using OrganizeMediaGUI.UserSettings;

namespace PictureOrganizerTests
{
    [TestClass]
    public class UnitTest1
    {
        static UnitTest1()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        [TestMethod]
        public void TestSettingsManagement()
        {
            SettingsManager manager = new SettingsManager();

            var initSettings = manager.InitializedSettings;

            var photoScreen = initSettings.Screens.Where(scr => scr.Name.Equals(Screen.OrganizePhotoScreenName)).FirstOrDefault();

            var searchFolderSetting = photoScreen.SettingsForScreen.Where(s => s.Name.Equals("SearchFolder")).FirstOrDefault();

            searchFolderSetting.Value = @"c:\users\dharmesh\pictures\";


            var retVal = manager.SaveSettings(initSettings);

            Assert.AreEqual(true, retVal);

            var deserializedSettingValue = manager.GetSettingValue(Screen.OrganizePhotoScreenName, "SearchFolder");


            Assert.AreEqual(searchFolderSetting.Value, deserializedSettingValue);



        }

        [TestMethod]
        public void CopyFilesTest()
        {
            IMediaOrganizer photoOrganizer = new PhotoOrganizer();
            List<string> files = new List<string>();

            files.Add(@"F:\DCIM\100D3300\DSC_8218.JPG");
            files.Add(@"F:\DCIM\100D3300\DSC_8217.JPG");
            files.Add(@"F:\DCIM\100D3300\DSC_8216.JPG");

            var result = photoOrganizer.CopyMedia(files, @"c:\temp\test");

            Assert.AreEqual(3, result.ResultValue);

        }

        [TestMethod]
        public void TestMethod1()
        {

              /// <param name="from">@"C:\Users\Dharmesh\Pictures\2016\M8\"</param>
        /// <param name="to">@"C:\Users\Dharmesh\Pictures\2015\Iphone\IPhonePictures\"</param>
        /// <param name="videoFrom">@"C:\Users\Dharmesh\Pictures\2015\Iphone\DCIM"</param>
        /// <param name="videoTo"> @"C:\Users\Dharmesh\Pictures\2015\Iphone\IphoneVideos\"</param>
        /// 
            //@"C:\Users\Dharmesh\Pictures\"
            var from = @"C:\Users\Dharmesh\Pictures\Test\";
            var to = @"C:\Users\Dharmesh\Pictures\";
            IMediaOrganizer photoOrganizer = new PhotoOrganizer();

            var newFilesToCopy = photoOrganizer.GetListOfNewMediaMissingInToFolder(from, to);

            Assert.AreEqual(0, newFilesToCopy.Errors.Count);

            //Assert.Equals(0, pvo.BackupPictures());
        }
    }
}
