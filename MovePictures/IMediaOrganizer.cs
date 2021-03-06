﻿using System;
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
        ListResult<string> GetDups(string fromFolder, IList<string> ignoreFolders);
        ScalarResult<int> CopyMedia(IList<string> fromFiles, string toFolder);
        ScalarResult<int> GetMediaCount(string folder);
    }

    public abstract class Result
    {
        public IList<string> Errors{ get; set; }
        public IList<string> Logs { get; set; }


        public void AddErrorFormat(string format,params object[] arguments)
        {
            ListAddFormat(Errors, format, arguments);
        }

        public void AddLogFormat(string format, params object[] arguments)
        {
            ListAddFormat(Logs, format, arguments);
        }

        private void ListAddFormat(IList<string> list,string format, params object[] arguments)
        {
            if (list != null)
            {
                list.Add(string.Format(format, arguments));
            }
        }
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
