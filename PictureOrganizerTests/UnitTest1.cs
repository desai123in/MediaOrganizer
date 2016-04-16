using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrganizeMedia;
using OrganizeMedia.Photo;
using System.Collections.Generic;
using log4net;

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

            IList<string> newFilesToCopy = photoOrganizer.GetListOfNewMediaMissingInToFolder(from, to);


            //Assert.Equals(0, pvo.BackupPictures());
        }
    }
}
