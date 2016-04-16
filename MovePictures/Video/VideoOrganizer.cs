using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizeMedia.Photo
{
    public class VideoOrganizer:IMediaOrganizer
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ListResult<string> GetListOfNewMediaMissingInToFolder(string fromFolder, string toFolder)
        {
            throw new NotImplementedException();
        }


        public ListResult<string> GetDups(string folder)
        {
            throw new NotImplementedException();
        }

        public ScalarResult<int> CopyMedia(List<string> fromFiles, string toFolder)
        {
            throw new NotImplementedException();
        }

        public string SearchFolder
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
