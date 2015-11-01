using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolHelper
{
   public static class TMS
    {
        //瓦片位置转经度 x 为图片位置相对全图大小的百分数
       public static double BlockToLongitude(int x, int zoom)
        {
            var re = x / Math.Pow(2.0, zoom) * 360.0 - 180;
            return re;
        }
        //瓦片位置转纬度 y 为图片位置相对全图大小的百分数
       public static double BlockToLatitude(int y, int zoom)
        {
            var n = Math.PI - (2.0 * Math.PI * y) / Math.Pow(2.0, zoom);
            var re = Math.Atan(Math.Sinh(n)) / Math.PI * 180;
            return re;
        }

        //经度转瓦片位置像素
       public static double LongitudeToBlock(double x, int zoom)
        {
            double blockpx = (double)((x + 180.0) / 360.0 * Math.Pow(2.0, zoom)); 
            return blockpx;
        }
        //纬度转瓦片位置像素
       public static double LatitudeToBlock(double y, int zoom)
        {

            double blockpy =(double) ((1.0 - Math.Log(Math.Tan(y * Math.PI / 180.0) +
                1.0 / Math.Cos(y * Math.PI / 180.0)) / Math.PI) / 2.0 * Math.Pow(2.0, zoom));
            return blockpy;
        }
    }
}
