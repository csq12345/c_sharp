using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMap.ToolHelper
{
    public static class zoomlevel
    {
        private static List<Zoom> zooms1 = new List<Zoom>()
        {
new Zoom(){level=4,maxX=15,maxY=15},
new Zoom(){level=5,maxX=31,maxY=31},
new Zoom(){level=6,maxX=63,maxY=63},
new Zoom(){level=7,maxX=127,maxY=127},
new Zoom(){level=8,maxX=255,maxY=255},
new Zoom(){level=9,maxX=511,maxY=511},
new Zoom(){level=10,maxX=1023,maxY=1023},
new Zoom(){level=11,maxX=2047,maxY=2047},
new Zoom(){level=12,maxX=4095,maxY=4095},
new Zoom(){level=13,maxX=8191,maxY=8191},
new Zoom(){level=14,maxX=16383,maxY=16383},
new Zoom(){level=15,maxX=32767,maxY=32767},
new Zoom(){level=16,maxX=65535,maxY=65535},
new Zoom(){level=17,maxX=131071,maxY=131071},
new Zoom(){level=18,maxX=262143,maxY=262143},
new Zoom(){level=19,maxX=524287,maxY=524287},
new Zoom(){level=20,maxX=1048575,maxY=1048575},
new Zoom(){level=21,maxX=2097151,maxY=2097151},

        };



        public static Zoom GetLevel(int zoom)
        {

            string url1 = "http://mt0.google.cn/vt?pb=!1m4!1m3!1i@Z!2i@X!3i@Y!2m3!1e0!2sm!3i323191379!3m9!2szh-Hans-CN!3sCN!5e78!12m1!1e47!12m3!1e37!2m1!1ssmartmaps!4e0";
            string ull2 = "http://mt1.google.cn/vt?pb=!1m4!1m3!1i@Z!2i@X!3i@Y!2m3!1e0!2sm!3i323000000!3m9!2szh-Hans-CN!3sCN!5e78!12m1!1e47!12m3!1e37!2m1!1ssmartmaps!4e0";
            Zoom zm = zooms1.Find(z => z.level == zoom);
            if (zm != null)
            {
                zm.url = ull2.Replace("@Z", zoom.ToString());
                return zm;
            }
            return null;
        }

    }


    public class Zoom
    {
        public int level { get; set; }
        public int maxX { get; set; }
        public int maxY { get; set; }
        public string url { get; set; }
    }
}
