﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolHelper
{
    public static class zoomlevel
    {
        public static Zoom GetLevel(int zoom,MapType mt)
        {

            string url1 = "http://mt0.google.cn/vt?pb=!1m4!1m3!1i@Z!2i@X!3i@Y!2m3!1e0!2sm!3i323191379!3m9!2szh-Hans-CN!3sCN!5e78!12m1!1e47!12m3!1e37!2m1!1ssmartmaps!4e0";
           //平面地图
            string url2 = "http://mt1.google.cn/vt?pb=!1m4!1m3!1i@Z!2i@X!3i@Y!2m3!1e0!2sm!3i323000000!3m9!2szh-Hans-CN!3sCN!5e78!12m1!1e47!12m3!1e37!2m1!1ssmartmaps!4e0";
           //卫星地图
            string url3 = "http://mt0.google.cn/vt?lyrs=s@186&hl=zh-Hans-CN&gl=CN&x=@X&y=@Y&z=@Z";
                         
            //http://mt0.google.cn/vt?lyrs=s@186&hl=zh-Hans-CN&gl=CN&x=210&y=98&z=8&token=94681
            //卫星地图线路
            string url4 = "http://mt0.google.cn/vt?pb=!1m5!1m4!1i@Z!2i@X!3i@Y!4i256!2m3!1e0!2sm!3i325000000!3m9!2szh-Hans-CN!3sCN!5e78!12m1!1e50!12m3!1e37!2m1!1ssmartmaps!4e0";
            //地形图
            string url5 =
                "http://mt1.google.cn/vt?pb=!1m5!1m4!1i@Z!2i@X!3i@Y!4i256!2m3!1e4!2st!3i132!2m3!1e0!2sr!3i325150060!3m9!2szh-Hans-CN!3sCN!5e78!12m1!1e63!12m3!1e37!2m1!1ssmartmaps!4e0";
            //Zoom zm = zooms1.Find(z => z.level == zoom);

            string url = url2;
            switch (mt)
            {
                case MapType.pm:
                {
                    url = url2;
                }
                    break;
                case MapType.wx:
                    {
                        url = url3;
                    }
                    break;
                case MapType.wxxl:
                    {
                        url = url4;
                    }
                    break;
                case MapType.dx:
                    {
                        url = url5;
                    }
                    break;
            }
            
            Zoom zm=new Zoom(){level = zoom,maxX = (int)Math.Pow(2,zoom)-1,maxY = (int)Math.Pow(2,zoom)-1};
            if (zm != null)
            {
                zm.url = url.Replace("@Z", zoom.ToString());
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


    public enum MapType
    {
        /// <summary>
        /// 平面图
        /// </summary>
        pm=0,
        /// <summary>
        /// 卫星图
        /// </summary>
        wx=1,
        /// <summary>
        /// 卫星线路
        /// </summary>
        wxxl=2,
        /// <summary>
        /// 地形图
        /// </summary>
        dx=3

    }
}