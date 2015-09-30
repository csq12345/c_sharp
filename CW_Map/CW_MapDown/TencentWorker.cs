using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW_MapDown
{
    //腾讯地图url计算公式
    //URL = z  /  Math.Floor(x / 16.0)  / Math.Floor(y / 16.0) / x_y.png

    public class TencentWorker : AMapWorker, IMapWorker
    {
        /// <summary>
        /// 服务根地址
        /// </summary>
        private string urlspace = @"http://p1.map.gtimg.com/maptilesv3/";


        private DownLoader downLoader;

        public TencentWorker()
        {
            downLoader = new DownLoader();
        }

        /// <summary>
        /// 开始下载 返回最大步进值
        /// </summary>
        /// <param name="zoomlevel"></param>
        /// <param name="savedir"></param>
        /// <returns></returns>
        public int StartDown(int zoomlevel, string savedir)
        {
            if (savedir.LastIndexOf("\\") != savedir.Length)
            {
                savedir = savedir + "\\";
            }

            int m = 15;//经
            int p = 15;//纬

            RunDown(savedir, zoomlevel, m, p);
            return m;
        }

        /// <summary>
        /// 重写父类下载地图
        /// </summary>
        /// <returns></returns>
        protected override bool Down(string savedir, int level, int m, int p)
        {

            string imgadd = "";
            string savepath = "";

            for (int x = 0; x <= m; x++)
            {
               
                for (int y = 0; y <= p; y++)
                {
                    imgadd = urlspace + GetDirectPath(level, x, y) + "//" + x + "_" + y + ".png";
                    savepath = savedir + GetDirectPath(level, x, y) + "//" + x + "_" + y + ".png";
                    downLoader.DownMapImg(imgadd, savepath);
                }
                downBackgroundWorker.ReportProgress(x);
            }
            return true;
        }


        public string GetDirectPath(int zoomlevel, int x, int y)
        {
            string path = zoomlevel + "//"
                          + Math.Floor(x / 16.0) + "//"
                          + Math.Floor(y / 16.0);
            return path;
        }





    }
}
