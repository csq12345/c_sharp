using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;

namespace MyMap.ToolHelper
{
    public class DownTool
    {

        Queue<DownModel> urls = new Queue<DownModel>();
        public List<string> DownStart(int x1, int x2, int y1, int y2)
        {


            string urltemplate = "http://mt.google.cn/vt/pb=!1m4!1m3!1i4!2i@X!3i@Y!2m3!1e0!2sm!3i323191379!3m10!2szh-CN!3scn!5e1105!12m4!1e68!2m2!1sset!2sRoadmap!12m1!1e38!4e0?authuser=0";
            string url = "";

            urls.Clear();
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    url = urltemplate.Replace("@X", x.ToString()).Replace("@Y", y.ToString());
                    DownModel dm = new DownModel();
                    dm.Url = url;
                    dm.Fielname = "z_" + x + "_" + y + ".png";
                    urls.Enqueue(dm);
                }
            }
          WebClient webClient=  GetWebClient();
           
            if (urls.Count > 0)
            {
                DownModel dm = urls.Dequeue();
                Uri uri = new Uri(dm.Url);
               
                webClient.DownloadFileAsync(uri, dm.Fielname, dm);
                //webClient.DownloadDataAsync(uri,dm);

                //MemoryStream ms = DoRequest(uri, dm);



            }
            return null;
        }

        void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            WebClient webClient = (WebClient) sender;
                    webClient.DownloadDataCompleted -= webClient_DownloadDataCompleted;
                    webClient.DownloadFileCompleted -= webClient_DownloadFileCompleted;
            if (e.Error == null)
            {
                if (urls.Count > 0)
                {
                    DownModel dm = urls.Dequeue();
                    Uri uri = new Uri(dm.Url);
                   
                    webClient = GetWebClient();
                    webClient.DownloadFileAsync(uri,dm.Fielname,dm);
                }
            }
            else
            {
                
            }
        }

        void webClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            WebClient webClient = (WebClient)sender;
            webClient.DownloadDataCompleted -= webClient_DownloadDataCompleted;
            webClient.DownloadFileCompleted -= webClient_DownloadFileCompleted;
            if (e.Error == null)
            {
                if (urls.Count > 0)
                {
                    DownModel dm = urls.Dequeue();
                    Uri uri = new Uri(dm.Url);

                    webClient = GetWebClient();
                    webClient.DownloadDataAsync(uri, dm);
                }
            }
        }

        MemoryStream DoRequest(Uri uri, DownModel dm)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

            webRequest.Accept="text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            webRequest.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            webRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            webRequest.Headers.Add("Cache-Control", "no-cache");

            webRequest.KeepAlive = true;
            webRequest.Headers.Add("DNT", "1");
            webRequest.Host = "mt0.google.cn";
            webRequest.Headers.Add("Pragma", "no-cache");
            webRequest.UserAgent =
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.76 Safari/537.36";
        
            webRequest.Headers.Add("X-Client-Data", "CJW2yQEIpbbJAQiptskBCMS2yQEI7ojKAQ==");
            WebResponse webResponse = webRequest.GetResponse();
            Stream stream = webResponse.GetResponseStream();
            MemoryStream ms = new MemoryStream();

            byte[] buffBytes = new byte[1000];
            int len = 0;
            while (true)
            {
                len = stream.Read(buffBytes, 0, 1000);
                if (len > 0)
                {
                    ms.Write(buffBytes, 0, len);
                }
                else
                {
                    break;
                }
            }
            return ms;

        }

        WebClient GetWebClient()
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            webClient.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            webClient.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            webClient.Headers.Add("Cache-Control", "no-cache");

            webClient.Headers.Add("DNT", "1");
            webClient.Headers.Add("Host", "mt0.google.cn");
            webClient.Headers.Add("Pragma", "no-cache");
            webClient.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.76 Safari/537.36");
            webClient.DownloadDataCompleted += webClient_DownloadDataCompleted;
            webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
            return webClient;
        }
    }
}
