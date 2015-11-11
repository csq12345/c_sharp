using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolHelper
{
    public class DownTool
    {
        private List<Task> taskpool = new List<Task>();
        Queue<DownModel> urls = new Queue<DownModel>();
        private int downcount = 0;

        /// <summary>
        /// 已完成下载的
        /// </summary>
        Queue<DownModel> completeDownModels = new Queue<DownModel>();
        /// <summary>
        /// 是否使用批量保存模式
        /// </summary>
        private bool isMulSave = false;
        /// <summary>
        ///下载是否运行
        /// </summary>
        bool downisRun = false;
        /// <summary>
        /// 创建队列是否运行
        /// </summary>
        bool addqueueisRun = false;
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

        private int _endx = 0;
        private int _endy = 0;
        private int _zoom = 0;
        private string _directorypath = "";
        private int _threadnumber = 0;
        private int _passstartx = 0;
        private int _passstarty = 0;
        private int _passendx = 0;
        private int _passendy = 0;
        private MapType _mt;
        private Task downtask = null;

        private Task saveTask = null;
        /// <summary>
        /// 插入队列的计数
        /// </summary>
        private int inputqueuecount = 0;
        public void DownStart(MapType mt, int startx, int endx, int starty, int endy,
            int zoom, string directorypath, int threadnumber,
            int passstartx, int passstarty, int passendx, int passendy, bool savemode = false)
        {
            saveInfo = new SaveInfo() { level = zoom, stopx = 0, stopy = 0 };
            saveInfo.savepath = directorypath + "/" + (int)mt;
            threacount = threadnumber;

            _endx = endx;
            _endy = endy;
            _zoom = zoom;
            directorypath = _directorypath;
            _passstartx = passstartx;
            _passstarty = passstarty;

            _mt = mt;
            //passx = x1;
            //passy = y1;
            isMulSave = savemode;
            if (isMulSave)
            {
                saveTask = CreateSaveTask();
            }

            downtask = CreateDownTask();




            CreateThread(startx, endx, starty, endy, passstartx, passstarty, passendx, passendy);
        }

        /// <summary>
        /// 创建队列线程
        /// </summary>
        /// <param name="startx"></param>
        /// <param name="starty"></param>
        public void CreateThread(int startx, int endx, int starty, int endy,
            int passstartx, int passstarty, int passendx, int passendy)
        {



            Thread ttt = new Thread(new ParameterizedThreadStart(CreateQueue));
            ttt.Start(new
            {
                startx = startx,
                endx = endx,
                starty = starty,
                endy = endy,
                passstartx = passstartx,
                passstarty = passstarty,
                passendx = passendx,
                passendy = passendy
            });


        }
        /// <summary>
        /// 由线程调用的 创建队列方法的封装
        /// </summary>
        /// <param name="obj"></param>
        void CreateQueue(object obj)
        {
            dynamic objvar = obj;
            int startx = objvar.startx;
            int endx = objvar.endx;
            int starty = objvar.starty;
            int endy = objvar.endy;
            int passstartx = objvar.passstartx;
            int passstarty = objvar.passstarty;
            int passendx = objvar.passendx;
            int passendy = objvar.passendy;

            addqueueisRun = true;
            CreateQueue(startx, starty, endx, endy,
                _zoom, _mt,
                passstartx, passstarty, passendx, passendy);
        }



        /// <summary>
        /// 创建队列方法
        /// </summary>
        /// <param name="startx"></param>
        /// <param name="starty"></param>
        /// <param name="endx"></param>
        /// <param name="endy"></param>
        /// <param name="zoom"></param>
        /// <param name="mt"></param>
        private void CreateQueue(int startx, int starty, int endx, int endy,
            int zoom, MapType mt,
            int passstartx, int passstarty, int passendx, int passendy)
        {
            try
            {
                if (passendx == 0)
                {
                    passendx = endx;
                }
                if (passendy == 0)
                {
                    passendy = endy;
                }
                inputqueuecount = 0;



                while (addqueueisRun)
                {
                    if (passstartx <= endx || passstarty <= endy)
                    {
                        if (urls.Count < 100)
                        {
                            OnPrecessStatuEvent("队列不足200 继续创建。" + urls.Count);

                            onprecessStatu("创建队列 开始于" + passstartx + "X" + passstarty);
                            Zoom zm = zoomlevel.GetLevel(zoom, mt);
                            if (endx > zm.maxX || endy > zm.maxY)
                            {
                                return;
                            }
                            string url = "";
                            int partnum = 0;
                            string partDoc = "";

                            for (int x = startx; x <= endx; x++)
                            {
                                for (int y = starty; y <= endy; y++)
                                {
                                    if (passstartx > 0)
                                    {
                                        x = passstartx;
                                        passstartx = 0;
                                    }
                                    if (passstarty > 0)
                                    {
                                        y = passstarty;
                                        passstarty = 0;
                                    }



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
                                    inputqueuecount++;

                                    if (x == passendx && y == passendy)
                                    {

                                    }
                                    if (x == passendx && y == passendy)
                                    {
                                        //开始数大于结束数 停止队列
                                        passstartx = endx + 1;
                                        passstarty = endy + 1;
                                        x = endx + 1;//设置x值大于截止值 则下次循环将不执行
                                        y = endy + 1;
                                        onprecessStatu("队列 x y 等于结束数 " + passendx + "x" + passendy);
                                        if (inputqueuecount <= 100 && downisRun == false)
                                        {
                                            BeginDown();//开始下载
                                        }
                                    }
                                    else
                                    {

                                        if (urls.Count > 100 && downisRun == false)
                                        {
                                            BeginDown();//开始下载
                                        }

                                        if (urls.Count > 200)
                                        {
                                            passstartx = x;
                                            passstarty = y;
                                            onprecessStatu("队列 " + urls.Count + " 停止入列。" + passstartx + "X" + passstarty + " 入列总数" + inputqueuecount);
                                            x = endx + 1;//设置x值大于截止值 则下次循环将不执行
                                            y = endy + 1;
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            Thread.Sleep(1000);

                        }
                    }
                    else
                    {
                        addqueueisRun = false;
                        onprecessStatu("队列 创建完成 停止入列。" + passstartx + "X" + passstarty + " 入列总数" + inputqueuecount);
                    }
                }
            }
            catch (Exception ex)
            {

                onprecessStatu(ex.Message+ " "+ ex.StackTrace);
            }
        }


        public void BeginDown()
        {
            if (downisRun == false)
            {


                if (downtask != null)
                {
                    downisRun = true;
                    downtask.Start();
                    if (isMulSave)
                    {
                        if (saveTask != null)
                        {

                            saveTask.Start();
                        }
                        else
                        {
                            onprecessStatu("保存线程未初始化");
                        }
                    }

                }
                else
                {
                    onprecessStatu("下载线程未初始化");
                }

            }

        }





        /// <summary>
        /// 创建现在任务线程
        /// </summary>
        /// <returns></returns>
        Task CreateDownTask()
        {
            Task t = new Task(() =>
             {
                 try
                 {
                     if (urls.Count > 0)
                     {
                         downcount = 0;
                         downisRun = true;
                         for (int i = 0; i < threacount; i++)
                         {
                             if (NextQuery())
                             {
                                 tc.allThreadNumber++;
                             }
                         }

                         //webClient.DownloadDataAsync(uri,dm);
                         //MemoryStream ms = DoRequest(uri, dm);
                     }
                 }
                 catch (Exception ex)
                 {

                     onprecessStatu(ex.Message + " " + ex.StackTrace);
                     DownStop();
                 }

             }
          );

            return t;
        }

        Task CreateSaveTask()
        {
            Task t = new Task(() =>
            {
                try
                {


                    int emptycount = 0;//保存队列空计数
                    int savecount = 0;
                    while (true)
                    {
                        Thread.Sleep(2000);

                        if (completeDownModels.Count > 100)//这里只处理大于100的队列 如果不大于100 则由最后一次空计数时保存
                        {



                            emptycount = 0;//重置空计数
                            while (true)
                            {
                                if (completeDownModels.Count > 0)
                                {
                                    DownModel dm = completeDownModels.Dequeue();
                                    if (dm != null)
                                    {
                                        File.WriteAllBytes(dm.Fielname, dm.DataBytes);
                                        //dm.Dispose();
                                        savecount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            OnPrecessStatuEvent("已保存：" + savecount);


                        }
                        else
                        {
                            if (!downisRun)
                            {


                                emptycount++;
                                OnPrecessStatuEvent("保存队列空：" + emptycount);
                                if (emptycount > 10) //如果保存队列十次空 则表示没有要保存的信息
                                {

                                    while (true)
                                    {
                                        if (completeDownModels.Count > 0)
                                        {
                                            DownModel dm = completeDownModels.Dequeue();
                                            if (dm != null)
                                            {
                                                File.WriteAllBytes(dm.Fielname, dm.DataBytes);
                                                //dm.Dispose();
                                                savecount++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                //下载还在进行 不累加空保存计数
                            }
                        }

                    }

                    //保存队列完成  触发事件
                    OnSaveCompleteEventHandle(savecount);
                }
                catch (Exception ex)
                {
                    DownStop();
                    onprecessStatu(ex.Message+" "+ex.StackTrace);

                }
            });
            return t;
        }

        public void DownStop()
        {
            addqueueisRun = false;
            //isRun = false;
            lock (urls)
            {
                urls.Clear();
            }
        }
        bool NextQuery()
        {
            try
            {
                if (urls.Count > 0)
                {
                    DownModel dm = urls.Dequeue();
                    if (dm != null)
                    {
                        Uri uri = new Uri(dm.Url);
                        WebClient webClient = GetWebClient();
                        if (isMulSave)
                        {
                            webClient.DownloadDataAsync(uri, dm);

                        }
                        else
                        {
                            webClient.DownloadFileAsync(uri, dm.Fielname, dm);
                        }


                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                onprecessStatu(ex.Message + " " + ex.StackTrace);
                DownStop();
                return false;
            }



        }

        void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                DownModel dm = (DownModel)e.UserState;
                if (e.Error == null)
                {
                }
                else
                {
                    OnPrecessStatuEvent("错误。" + e.Error.Message + " " + dm.Url);
                    errdm.Add(dm);
                }
                DownCompleteProcess(dm);
            }
            catch (Exception ex)
            {
                DownStop();
                onprecessStatu(ex.Message + " " + ex.StackTrace);
            }
            ((WebClient)sender).DownloadFileCompleted -= webClient_DownloadFileCompleted;
        }

        void webClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                DownModel dm = (DownModel)e.UserState;
                if (e.Error == null)
                {

                    dm.DataBytes = e.Result;
                    lock (completeDownModels)
                    {
                        completeDownModels.Enqueue(dm); 
                    }
                   
                }
                else
                {

                    OnPrecessStatuEvent("错误。" + e.Error.Message + " " + dm.Url);
                    errdm.Add(dm);
                }
                DownCompleteProcess(dm);
            }
            catch (Exception ex)
            {
                DownStop();
                onprecessStatu(ex.Message+" "+ex.StackTrace);
            }



            ((WebClient)sender).DownloadDataCompleted -= webClient_DownloadDataCompleted;

        }


        void DownCompleteProcess(DownModel dm)
        {
            try
            {
                downcount++;
                if (downcount % 20 == 0)
                {
                    if (downcount % 500 == 0)
                    {
                        if (dm.x > saveInfo.stopx)
                        {
                            saveInfo.stopx = dm.x;
                        }
                        if (dm.y > saveInfo.stopy)
                        {
                            saveInfo.stopy = dm.y;
                        }
                        //File.WriteAllText(saveInfo.savepath + "/saveinfo.inf",
                        //saveInfo.level + "," + saveInfo.stopx + "," + saveInfo.stopy);
                    }

                    OnPrecessEvent(downcount);
                }

                //完成下载 触发事件
                if (urls.Count > 0 && NextQuery())
                {

                }
                else
                {
                    lock (tc)
                    {
                        if (tc.allThreadNumber > 0)
                        {
                            tc.allThreadNumber--;
                            OnPrecessStatuEvent("线程完成 剩余:" + tc.allThreadNumber);

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
                                downisRun = false;
                                //File.WriteAllText(saveInfo.savepath + "/saveinfo.inf",
                                //    saveInfo.level + "," + saveInfo.stopx + "," + saveInfo.stopy);
                                OnCompleteEvent(downcount, isMulSave);
                            }
                        }
                        else
                        {
                            OnPrecessStatuEvent("线程全部完成");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                DownStop();
                onprecessStatu(ex.Message + " " + ex.StackTrace);
            }
        }


        [Obsolete]
        MemoryStream DoRequest(Uri uri, DownModel dm)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            webRequest.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            webRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            webRequest.Headers.Add("Cache-Control", "max-age=0");

            webRequest.KeepAlive = true;
            webRequest.Headers.Add("DNT", "1");
            webRequest.Host = "mt0.google.cn";
            //webRequest.Headers.Add("Pragma", "no-cache");
            webRequest.UserAgent =
                @"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) 
Chrome/28.0.1500.95 Safari/537.36 SE 2.X MetaSr 1.0";

            //webRequest.Headers.Add("X-Client-Data", "CJW2yQEIpbbJAQiptskBCMS2yQEI7ojKAQ==");
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

        /// <summary>
        /// 获取要下载范围的大小
        /// </summary>
        /// <param name="startx"></param>
        /// <param name="starty"></param>
        /// <param name="endx"></param>
        /// <param name="endy"></param>
        /// <param name="passx"></param>
        /// <param name="passy"></param>
        /// <returns></returns>
        public int GetDownSize(int startx, int endx, int starty, int endy, int passx, int passy)
        {

            //((x2 - x1 + 1) * (y2 - y1 + 1)
            //    - ((sx - x1) * (y2 - y1 + 1) + (sy - y1)))

            return ((endx - startx + 1) * (endy - starty + 1)
              - ((passx - startx) * (endy - starty + 1) + (passy - starty)));
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
        public delegate void OnComplete(int AllCompleteCount, bool ismulsave);

        public event OnComplete oncomplete;

        public void OnCompleteEvent(int AllCompleteCount, bool ismulsave)
        {
            if (oncomplete != null)
            {
                oncomplete.Invoke(AllCompleteCount, ismulsave);
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



        public delegate void OnSaveCompleteEvent(int savecount);
        /// <summary>
        ///  保存完成
        /// </summary>	
        public event OnSaveCompleteEvent OnSaveComplete;
        public void OnSaveCompleteEventHandle(int savecount)
        {
            if (OnSaveComplete != null)
            {
                OnSaveComplete(savecount);
            }
        }
        #endregion


    }
}
