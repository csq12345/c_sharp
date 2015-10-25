using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyMap.ToolHelper
{
    public class DownTool
    {

        Queue<DownModel> urls = new Queue<DownModel>();
        private int downcount = 0;
        bool isRun = false;

        private ThreadCount tc = new ThreadCount();
        class ThreadCount
        {
            public int allThreadNumber
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 下载异常的地址
        /// </summary>
        public List<DownModel> errdm = new List<DownModel>();

        private int threacount = 0;
        private SaveInfo saveInfo;


        private int passx = 0;
        private int passy = 0;
        private int endx = 0;
        private int endy = 0;
        private int _zoom = 0;
        private string _directorypath = "";
        private int _threadnumber = 0;
        private int _startx = 0;
        private int _starty = 0;
        private MapType _mt;

        public void DownStart(MapType mt, int x1, int x2, int y1, int y2, int zoom, string directorypath, int threadnumber, int startx, int starty)
        {
            saveInfo = new SaveInfo() { level = zoom, stopx = 0, stopy = 0 };
            saveInfo.savepath = directorypath + "/" + (int)mt;
            threacount = threadnumber;
            tc.allThreadNumber = threadnumber;
            endx = x2;
            endy = y2;
            _zoom = zoom;
            directorypath = _directorypath;
            _startx = startx;
            _starty = starty;
            _mt = mt;
            //passx = x1;
            //passy = y1;
            CreateThread(x1, y1);
        }

        public void CreateThread(int x1, int y1)
        {
            Thread ttt = new Thread(new ParameterizedThreadStart(Create));
            ttt.Start(new{x1=x1, y1=y1});
        }
        public void Create(object obj)
        {
            dynamic objvar = obj;
            int x1=objvar.x1;
            int y1 = objvar.y1;
            if (urls.Count < 200)
            {
                OnPrecessStatuEvent("队列不足200 继续创建。" + urls.Count);

                onprecessStatu("创建队列 开始于" + x1 + "X" + y1);
                Zoom zm = zoomlevel.GetLevel(_zoom, _mt);
                if (endx > zm.maxX || endy > zm.maxY)
                {
                    return;
                }
                string url = "";
                int partnum = 0;
                string partDoc = "";
                urls.Clear();
           
                for (int x = 0; x <= endx; x++)
                {
                    if (x1 > -1)
                    {
                        x = x1;
                        x1 = -1;
                    }
                    
                    for (int y = 0; y <= endy; y++)
                    {
                        if (y1 > -1)
                        {
                            y = y1+1;
                            y1 = -1;
                        }
                        if (x >= _startx && y >= _starty)
                        {
                            _startx = 0;
                            _starty = 0;
                            url = zm.url.Replace("@X", x.ToString()).Replace("@Y", y.ToString());
                            DownModel dm = new DownModel();
                            dm.Url = url;

                            dm.x = x;
                            dm.y = y;
                            partnum = (int)Math.Floor((double)((double)(x + 1) * (double)(zm.maxY + 1) / 1000));
                            //计算分部文件夹编号


                            partDoc = saveInfo.savepath + "/" + _zoom + "/" + _zoom + "_" + partnum;
                            if (!Directory.Exists(partDoc))
                            {
                                Directory.CreateDirectory(partDoc);
                            }

                            dm.Fielname = partDoc + "/" + _zoom + "_" + x + "_" + y + ".png";
                            urls.Enqueue(dm);
                            if (urls.Count > 100 && isRun == false)
                            {
                                BeginDown();
                            }
                            if (urls.Count > 1000)
                            {
                                passx = x;
                                passy = y;

                                onprecessStatu("队列 " + urls.Count + " 停止入列。" + passx + "X" + passy);
                                Create(new { x1 = passx, y1 = passy });
                            }
                        }
                    }
                }
            }
            else
            {
                Thread.Sleep(1000);
                Create(new { x1 = passx, y1 = passy });
            }
        }

        public void BeginDown()
        {
            if (urls.Count > 0)
            {
                downcount = 0;
                isRun = true;
                for (int i = 0; i < threacount; i++)
                {
                    NextQuery();
                }

                //webClient.DownloadDataAsync(uri,dm);
                //MemoryStream ms = DoRequest(uri, dm);
            }
        }


        public void DownStop()
        {
            //isRun = false;
            lock (urls)
            {
                urls.Clear();
            }
        }
        void NextQuery()
        {
            DownModel dm = urls.Dequeue();
            if (dm != null)
            {
                Uri uri = new Uri(dm.Url);
                WebClient webClient = GetWebClient();
                webClient.DownloadFileAsync(uri, dm.Fielname, dm);
                //   Task t = new Task(() =>
                //   {

                //   }
                //);

                //   t.Start();
            }


        }

        void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //WebClient webClient = (WebClient)sender;
            //webClient.DownloadDataCompleted -= webClient_DownloadDataCompleted;
            //webClient.DownloadFileCompleted -= webClient_DownloadFileCompleted;
            if (e.Error == null)
            {
            }
            else
            {
                DownModel dm = (DownModel)e.UserState;
                OnPrecessStatuEvent("错误。" + e.Error.Message + " " + dm.Url);
                errdm.Add(dm);
            }

            downcount++;
            if (downcount % 20 == 0)
            {
                if (downcount % 500 == 0)
                {
                    DownModel dm = (DownModel)e.UserState;
                    if (dm.x > saveInfo.stopx)
                    {
                        saveInfo.stopx = dm.x;
                    }
                    if (dm.y > saveInfo.stopy)
                    {
                        saveInfo.stopy = dm.y;
                    }
                    File.WriteAllText(saveInfo.savepath + "/saveinfo.inf",
                                       saveInfo.level + "," + saveInfo.stopx + "," + saveInfo.stopy);
                }

                OnPrecessEvent(downcount);
            }

            //完成下载 触发事件
            if (urls.Count > 0)
            {
                NextQuery();
            }
            else
            {
                lock (tc)
                {


                    if (tc.allThreadNumber > 0)
                    {
                        tc.allThreadNumber--;
                        OnPrecessStatuEvent("线程完成 剩余:" + tc.allThreadNumber);
                        DownModel dm = (DownModel)e.UserState;
                        if (dm.x > saveInfo.stopx)
                        {
                            saveInfo.stopx = dm.x;
                        }
                        if (dm.y > saveInfo.stopy)
                        {
                            saveInfo.stopy = dm.y;
                        }

                        if (tc.allThreadNumber == 0)
                        {
                            OnPrecessStatuEvent("线程全部完成 触发完成事件");
                            isRun = false;
                            File.WriteAllText(saveInfo.savepath + "/saveinfo.inf",
                                saveInfo.level + "," + saveInfo.stopx + "," + saveInfo.stopy);
                            OnCompleteEvent(downcount);
                        }
                    }
                }

            }
            //}
            //else
            //{
            //    lock (tc)
            //    {
            //        if (tc.allThreadNumber > 0)
            //        {
            //            tc.allThreadNumber--;
            //            OnPrecessStatuEvent("线程完成 剩余:" + tc.allThreadNumber);
            //            DownModel dm = (DownModel)e.UserState;
            //            if (dm.x > saveInfo.stopx)
            //            {
            //                saveInfo.stopx = dm.x;
            //            }
            //            if (dm.y > saveInfo.stopy)
            //            {
            //                saveInfo.stopy = dm.y;
            //            }

            //            if (tc.allThreadNumber == 0)
            //            {
            //                OnPrecessStatuEvent("线程全部完成 触发完成事件");
            //                isRun = false;
            //                File.WriteAllText(saveInfo.savepath + "/saveinfo.inf",
            //                    saveInfo.level + "," + saveInfo.stopx + "," + saveInfo.stopy);
            //                OnCompleteEvent(downcount);
            //            }
            //        }
            //    }

            //}


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
                else
                {

                }
            }
        }

        MemoryStream DoRequest(Uri uri, DownModel dm)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
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

        #region 事件
        public delegate void OnComplete(int AllCompleteCount);

        public event OnComplete oncomplete;

        public void OnCompleteEvent(int AllCompleteCount)
        {
            if (oncomplete != null)
            {
                oncomplete.Invoke(AllCompleteCount);
                //oncomplete(AllCompleteCount);
            }
        }


        public delegate void OnPrecess(int AllCompleteCount);

        public event OnPrecess onprecess;

        public void OnPrecessEvent(int precesspart)
        {
            if (onprecess != null)
            {
                onprecess(precesspart);
            }
        }

        public delegate void OnPrecessStatu(string statu);

        public event OnPrecessStatu onprecessStatu;

        public void OnPrecessStatuEvent(string statu)
        {
            if (onprecess != null)
            {
                onprecessStatu(statu);
            }
        }

        #endregion


    }
}
