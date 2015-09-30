using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW_MapDown
{
    /// <summary>
    /// 抽象地图下载
    /// </summary>
    public abstract class AMapWorker
    {
        /// <summary>
        /// 抽象的下载方法 子类实现将如何下载地图数据
        /// </summary>
        /// <param name="savedir"></param>
        /// <param name="level"></param>
        /// <param name="m"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        protected abstract bool Down(string savedir, int level, int m, int p);
        /// <summary>
        /// 后台下载线程
        /// </summary>
        protected BackgroundWorker downBackgroundWorker;

        
        public delegate void dlgProgressChanged(int progressPercentage);
        /// <summary>
        /// 下载进度事件
        /// </summary>
        public event dlgProgressChanged ProgressChanged;

        public AMapWorker()
        {
            downBackgroundWorker = new BackgroundWorker();
            downBackgroundWorker.DoWork += downBackgroundWorker_DoWork;
            downBackgroundWorker.ProgressChanged += downBackgroundWorker_ProgressChanged;
            downBackgroundWorker.WorkerReportsProgress = true;
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        public void RunDown(string savedir, int level, int m, int p)
        {

            downBackgroundWorker.RunWorkerAsync(new object[] { savedir, level, m, p });
        }

        void downBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(e.ProgressPercentage);
            }
        }

        void downBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Array para = e.Argument as Array;
            string savedir = para.GetValue(0).ToString();
            int level = (int)para.GetValue(1);
            int m = (int)para.GetValue(2);
            int p = (int)para.GetValue(3);
            Down(savedir, level, m, p);
        }
    }
}
