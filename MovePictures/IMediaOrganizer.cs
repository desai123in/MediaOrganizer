using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizeMedia
{

    public interface IMediaOrganizer
    {
        string SearchFolder { get; set; }
        ListResult<string> GetListOfNewMediaMissingInToFolder(string fromFolder, string toFolder);
        ListResult<string> GetDups(string folder);
        ScalarResult<int> CopyMedia(List<string> fromFiles, string toFolder);
    }

    public abstract class Result
    {
        public IList<string> Errors{ get; set; }
        public IList<string> Logs { get; set; }

    }

    public class ListResult<T>:Result
    {
        public IList<T> ResultCollection { get; set; } 
        
    }

    public class ScalarResult<T>:Result
    {
        public T ResultValue { get; set; }
    }
}
