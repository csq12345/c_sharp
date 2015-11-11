using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolHelper
{
    public class DownModel:IDisposable
    {
        public string Url { get; set; }

        public string Fielname { get; set; }

        public int x { get; set; }
        public int y { get; set; }

        public byte[] DataBytes { get; set; }

        public void Dispose()
        {
            Url = null;
            Fielname = null; 
            DataBytes = null;
        }
    }
}
