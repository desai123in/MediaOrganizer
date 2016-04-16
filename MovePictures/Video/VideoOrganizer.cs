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

        public IList<string> GetListOfNewMediaMissingInToFolder(string fromFolder, string toFolder)
        {
            throw new NotImplementedException();
        }


        public IList<string> GetDups(string folder)
        {
            throw new NotImplementedException();
        }

        public int CopyMedia(List<string> fromFiles, string toFolder)
        {
            return 0;
        }
    }
}
