
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToolHelper;

namespace MultiTask
{
    /// <summary>
    /// DownGoogle.xaml 的交互逻辑
    /// </summary>
    public partial class WinDownGoogle : Window
    {
        private bool isrun = false;
        private DownTool downTool;
        /// <summary>
        ///是否是其他进程启动的
        /// </summary>
        bool isotherRun = false;

        bool actionselectchange = true;

        private int allcount = 0;

        public WinDownGoogle()
        {
            InitializeComponent();
            downTool = new DownTool();
            for (int i = 4; i <= 21; i++)
            {
                ComboBoxZoom.Items.Add(i);
            }
            ComboBoxZoom.SelectedIndex = 0;

            ComboBoxMapType.DisplayMemberPath = "name";
            ComboBoxMapType.SelectedValuePath = "value";
            ComboBoxMapType.Items.Add(new { name = "平面", value = MapType.pm });
            ComboBoxMapType.Items.Add(new { name = "卫星", value = MapType.wx });
            ComboBoxMapType.Items.Add(new { name = "地形", value = MapType.dx });
            ComboBoxMapType.SelectedIndex = 0;

            ComboBoxSaveMode.DisplayMemberPath = "name";
            ComboBoxSaveMode.SelectedValuePath = "value";
            ComboBoxSaveMode.Items.Add(new { name = "即时保存", value = 0 });
            ComboBoxSaveMode.Items.Add(new { name = "批量保存", value = 1 });
            ComboBoxSaveMode.SelectedIndex = 0;

            downTool.oncomplete += downTool_oncomplete;
            downTool.onprecess += downTool_onprecess;
            downTool.onprecessStatu += downTool_onprecessStatu;
            downTool.OnSaveComplete += downTool_OnSaveComplete;
            //获取开始截止参数
            string[] args = System.Environment.GetCommandLineArgs();
            //Title = string.Join(",", args);
            if (args.Count() == 12)
            {
                actionselectchange = false;

                TextBoxX1.Text = args[1];
                TextBoxX2.Text = args[2];
                TextBoxY1.Text = args[3];
                TextBoxY2.Text = args[4];

                TextBoxSatrtx.Text = args[5];
                TextBoxEndx.Text = args[6];
                TextBoxStarty.Text = args[7];
                TextBoxEndy.Text = args[8];

                int si = 0;
                int.TryParse(args[9], out si);
                ComboBoxZoom.SelectedIndex = si;




                int mti = 0;
                int.TryParse(args[10], out mti);
                ComboBoxMapType.SelectedIndex = mti;

                int savemode = 0;
                int.TryParse(args[11], out savemode);



                isotherRun = true;
                ButtonStartTasks.IsEnabled = false;//禁用开始多任务按钮
                ComboBoxZoom.IsEnabled = false;
                ButtonSS.IsEnabled = false;
                ComboBoxMapType.IsEditable = false;
                //ButtonSS_Click(null, null);//调用开始按钮


                int x1 = 0, x2 = 0, y1 = 0, y2 = 0, zoom = 0, t = 1, sx = 0, sy = 0, ex = 0, ey = 0;

                int.TryParse(TextBoxX1.Text, out x1);
                int.TryParse(TextBoxX2.Text, out x2);
                int.TryParse(TextBoxY1.Text, out y1);
                int.TryParse(TextBoxY2.Text, out y2);
                int.TryParse(TextBoxSatrtx.Text, out sx);
                int.TryParse(TextBoxStarty.Text, out sy);

                int.TryParse(TextBoxEndx.Text, out ex);
                int.TryParse(TextBoxEndy.Text, out ey);

                int.TryParse(TextBoxThreadNum.Text, out t);
                allcount = ((x2 - x1 + 1) * (y2 - y1 + 1));
                TextBoxAll.Text = allcount.ToString();
                zoom = int.Parse(ComboBoxZoom.SelectedItem.ToString());

                Labelzoom.Content = "[" + zoom + "]";

                ButtonReady_Click(null, null);//调用估算按钮

                MapType maptype = (MapType)ComboBoxMapType.SelectedValue;
                StartDown(maptype, x1, x2, y1, y2, zoom, t, sx, sy, ex, ey, savemode);
            }
        }

        void downTool_OnSaveComplete(int savecount)
        {
            ButtonSS.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (isotherRun)
                {
                    ShowInfo("完成保存1：" + savecount);
                }
                else
                {

                    ShowInfo("完成保存：" + savecount);
                }
            }));
        }

        void downTool_onprecessStatu(string statu)
        {
            ShowInfo(statu);
        }

        void ShowInfo(string info)
        {
            ListBoxStatu.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ListBoxStatu.Items.Count == 301)
                {
                    ListBoxStatu.Items.RemoveAt(300);
                }
                ListBoxStatu.Items.Insert(0, info);
            }), null);
        }

        private void ButtonSS_Click(object sender, RoutedEventArgs e)
        {
            if (isrun)
            {
                isrun = false;
                downTool.DownStop();
                ButtonSS.Content = "开始";

            }
            else
            {
                int x1 = 0, x2 = 0, y1 = 0, y2 = 0, zoom = 0, t = 1, sx = 0, sy = 0, ex = 0, ey = 0;

                int.TryParse(TextBoxX1.Text, out x1);
                int.TryParse(TextBoxX2.Text, out x2);
                int.TryParse(TextBoxY1.Text, out y1);
                int.TryParse(TextBoxY2.Text, out y2);
                int.TryParse(TextBoxSatrtx.Text, out sx);
                int.TryParse(TextBoxStarty.Text, out sy);

                int.TryParse(TextBoxEndx.Text, out ex);
                int.TryParse(TextBoxEndy.Text, out ey);

                int.TryParse(TextBoxThreadNum.Text, out t);
                TextBoxAll.Text = ((x2 - x1 + 1) * (y2 - y1 + 1)).ToString();
                zoom = int.Parse(ComboBoxZoom.SelectedItem.ToString());
                MapType maptype = (MapType)ComboBoxMapType.SelectedValue;

                StartDown(maptype, x1, x2, y1, y2, zoom, t, sx, sy, ex, ey,
                    (int)ComboBoxSaveMode.SelectedValue);

                isrun = true;
                ButtonSS.Content = "停止";
            }
        }

        void StartDown(MapType mt, int x1, int x2, int y1, int y2, int zoom, int threadnumber,
            int passstartx, int passstarty, int passendx, int passendy, int savemode)
        {

            downTool.DownStart(mt, x1, x2, y1, y2, zoom, "D:/temp/googlepic", threadnumber,
                passstartx, passstarty, passendx, passendy, savemode == 0 ? false : true);
        }


        void downTool_onprecess(int AllCompleteCount)
        {
            SetProcessValue(AllCompleteCount);
        }


        private void SetProcessValue(int value)
        {
            TextBoxcompletecount.Dispatcher.BeginInvoke(new Action(
                () =>
                {
                    TextBoxcompletecount.Text = value.ToString();

                    Title = (value * 100 / allcount) + "%";
                }
    )
                , null
            );
        }

        void downTool_oncomplete(int AllCompleteCount,bool ismulsave)
        {
            isrun = false;
            ButtonSS.Dispatcher.BeginInvoke(new Action(() =>
            {
                ButtonSS.Content = "开始";
                if (isotherRun)
                {
                    if (ismulsave)
                    {
                        
                    }
                    else
                    {
                         this.Close(); 
                    }
                  
                }
                else
                {
                    MessageBox.Show(this, "完成下载：" + AllCompleteCount + " 错误:" + downTool.errdm.Count);


                }
            }));


        }

        private void ComboBoxZoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (actionselectchange)
            {
                int z = (int)e.AddedItems[0];
                Zoom zm = zoomlevel.GetLevel(z, MapType.pm);
                TextBoxX2.Text = zm.maxX.ToString();
                TextBoxY2.Text = zm.maxX.ToString();
                TextBoxAll.Text = ((zm.maxX + 1) * (zm.maxY + 1)).ToString();
            }

        }

        private void ButtonConvertBlock_Click(object sender, RoutedEventArgs e)
        {



            double x = 0, y = 0;
            int z = 0;
            double.TryParse(TextBoxLongitude.Text, out x);
            double.TryParse(TextBoxLatitude.Text, out y);
            int.TryParse(ComboBoxZoom.SelectedItem.ToString(), out z);


            int bx = (int)TMS.LongitudeToBlock(x, z);
            int by = (int)TMS.LatitudeToBlock(y, z);

            TextBoxX.Text = bx.ToString();
            TextBoxY.Text = by.ToString();


        }
        public Point WorldToTilePos(double lon, double lat, int zoom)
        {
            Point p = new Point();
            p.X = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            p.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return p;
        }

        public Point TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            Point p = new Point();
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

            p.X = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            p.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            return p;
        }

        private void ButtonReady_Click(object sender, RoutedEventArgs e)
        {
            int x1 = 0, x2 = 0, y1 = 0, y2 = 0, zoom = 0, sx = 0, sy = 0, ex = 0, ey = 0;

            int.TryParse(TextBoxX1.Text, out x1);
            int.TryParse(TextBoxX2.Text, out x2);
            int.TryParse(TextBoxY1.Text, out y1);
            int.TryParse(TextBoxY2.Text, out y2);
            int.TryParse(TextBoxSatrtx.Text, out sx);
            int.TryParse(TextBoxStarty.Text, out sy);
            int.TryParse(TextBoxEndx.Text, out ex);
            int.TryParse(TextBoxEndy.Text, out ey);
            int startnum = downTool.GetDownSize(x1, x2, y1, y2, sx, sy);
            if (ex == 0)
            {
                ex = x2;
            }
            if (ey == 0)
            {
                ey = y2;
            }
            int endnum = downTool.GetDownSize(x1, x2, y1, y2, ex, ey);
            allcount = (startnum - endnum);
            TextBoxAll.Text = allcount.ToString();
        }

        private void ButtonConvertToxy_OnClick(object sender, RoutedEventArgs e)
        {
            double x = 0, y = 0;
            int z = 0;
            int.TryParse(ComboBoxZoom.SelectedItem.ToString(), out z);

            double.TryParse(TextBoxStartLatitute.Text, out x);
            double.TryParse(TextBoxStartLongitute.Text, out y);

            int bx = (int)TMS.LongitudeToBlock(x, z);
            int by = (int)TMS.LatitudeToBlock(y, z);

            TextBoxX1.Text = bx.ToString();
            TextBoxY2.Text = by.ToString();


            double.TryParse(TextBoxEndLatitute.Text, out x);
            double.TryParse(TextBoxEndLongitute.Text, out y);

            bx = (int)TMS.LongitudeToBlock(x, z);
            by = (int)TMS.LatitudeToBlock(y, z);

            TextBoxX2.Text = bx.ToString();
            TextBoxY1.Text = by.ToString();

            TextBoxSatrtx.Text = TextBoxX1.Text;
            TextBoxStarty.Text = TextBoxY1.Text;
        }
        List<Process> processes = new List<Process>();
        private bool isrunmtask = false;
        private void ButtonStartTasks_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (!isrunmtask)
                {
                    foreach (var item in processes)
                    {
                        item.Kill();
                    }
                    processes.Clear();
                    int startx = 0, endx = 0, starty = 0, endy = 0, zoom = 0, t = 1, sx = 0, sy = 0, ex = 0, ey = 0, tasknum = 0;

                    int.TryParse(TextBoxX1.Text, out startx);
                    int.TryParse(TextBoxX2.Text, out endx);
                    int.TryParse(TextBoxY1.Text, out starty);
                    int.TryParse(TextBoxY2.Text, out endy);

                    int.TryParse(TextBoxSatrtx.Text, out sx);
                    int.TryParse(TextBoxStarty.Text, out sy);

                    int.TryParse(TextBoxThreadNum.Text, out t);

                    int.TryParse(TextBoxEndx.Text, out ex);
                    int.TryParse(TextBoxEndy.Text, out ey);

                    int.TryParse(TextBoxTaskNumber.Text, out tasknum);


                    int startnum = downTool.GetDownSize(startx, endx, starty, endy, sx, sy);
                    if (ex == 0)
                    {
                        ex = endx;
                    }
                    if (ey == 0)
                    {
                        ey = endy;
                    }
                    int endnum = downTool.GetDownSize(startx, endx, starty, endy, ex, ey);
                    TextBoxAll.Text = (startnum - endnum).ToString();

                    //TextBoxAll.Text = ((endx - startx + 1) * (endy - starty + 1)).ToString();
                    zoom = ComboBoxZoom.SelectedIndex;
                    int maptypeindex = ComboBoxMapType.SelectedIndex;


                    int maxsize = (startnum - endnum) / tasknum;
                    int savemode = (int)ComboBoxSaveMode.SelectedValue;


                    Task task = new Task(() =>
                    {

                        int count = 0;

                        int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                        for (int x = startx; x <= endx; x++)
                        {
                            for (int y = starty; y <= endy; y++)
                            {
                                if (count == 0)
                                {
                                    x1 = x;
                                    y1 = y;
                                }
                                count++;
                                if (count == maxsize || (x == endx && y == endy))
                                {
                                    x2 = x;
                                    y2 = y;
                                    //启动一个任务
                                    Process p = new Process();
                                    ProcessStartInfo psi = new ProcessStartInfo("MultiTask.exe",
                                        startx + " " + endx + " " + starty + " " + endy + " " + x1 + " " + x2 + " " + y1 + " " + y2
                                        + " " + zoom + " " + maptypeindex + " " + savemode);
                                    p.StartInfo = psi;
                                    psi.WindowStyle = ProcessWindowStyle.Minimized;

                                    processes.Add(p);
                                    p.Start();
                                    count = 0;
                                }
                            }
                        }
                        int procount = processes.Count;

                        //foreach (var item in processes)
                        //{
                        //    item.Start();
                        //}
                    });
                    task.Start();
                    isrunmtask = true;
                    Labeltask.Content = "停止进程";
                }
                else
                {
                    foreach (var item in processes)
                    {
                        if (!item.HasExited)
                        {
                            item.Kill();
                        }

                    }
                    processes.Clear();
                    Labeltask.Content = "开始任务";
                    isrunmtask = false;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.StackTrace);
            }
        }
    }
}
