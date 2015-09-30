using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CW_MapDown
{
    /// <summary>
    /// 下载器 负责根据指定的url抓取数据
    /// </summary>
    class DownLoader
    {
        private WebClient downWebClient;

       public DownLoader()
        {
            downWebClient=new WebClient();
        }
        /// <summary>
        /// 下载指定地址的图片 返回保存的位置
        /// </summary>
        /// <param name="imageAddress"></param>
        /// <param name="savepath"></param>
        /// <returns></returns>
        public string DownMapImg(string  imageAddress,string savepath)
        {
            string dir = Path.GetDirectoryName(savepath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            byte[] dataBytes = downWebClient.DownloadData(imageAddress);
            File.WriteAllBytes(savepath,dataBytes);
            return savepath;
        }
    }
}
