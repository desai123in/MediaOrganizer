using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizeMedia
{

    public interface IMediaOrganizer
    {
        IList<string> GetListOfNewMediaMissingInToFolder(string fromFolder, string toFolder);
        IList<string> GetDups(string folder);
        int CopyMedia(List<string> fromFiles, string toFolder);
    }
}
