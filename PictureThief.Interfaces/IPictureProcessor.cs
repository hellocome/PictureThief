using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;



namespace PictureThief.Interfaces
{
    public interface IPictureProcessor
    {
        List<PageInfo> PageInfoList { get; }
        string Name { get; set; }
        DirectoryInfo SaveToDirectory { get; set; }
        bool UpdateMode { get; set; }

        void PicProcess();

        bool Load();
    }
}
