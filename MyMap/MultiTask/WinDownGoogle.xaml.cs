
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
            downTool.oncomplete += downTool_oncomplete;
            downTool.onprecess += downTool_onprecess;
            downTool.onprecessStatu += downTool_onprecessStatu;

            //获取开始截止参数
            string[] args = System.Environment.GetCommandLineArgs();
            Title = string.Join(",", args);
            if (args.Count() == 11)
            {
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
                isotherRun = true;
                ButtonStartTasks.IsEnabled = false;//禁用开始多任务按钮

                ButtonReady_Click(null,null);//调用估算按钮
                ButtonSS_Click(null, null);//调用开始按钮
            }
        }

        void downTool_onprecessStatu(string statu)
        {
            ListBoxStatu.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ListBoxStatu.Items.Count == 301)
                {
                    ListBoxStatu.Items.RemoveAt(300);
                }
                ListBoxStatu.Items.Insert(0, statu);
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
                downTool.DownStart(maptype, x1, x2, y1, y2, zoom, "D:/temp/googlepic", t, sx, sy, ex, ey);

                isrun = true;
                ButtonSS.Content = "停止";
            }
        }

        void downTool_onprecess(int AllCompleteCount)
        {
            SetProcessValue(AllCompleteCount);
        }


        void SetProcessValue(int value)
        {
            TextBoxcompletecount.Dispatcher.BeginInvoke(new Action(
                () => TextBoxcompletecount.Text = value.ToString()
                )
                , null
            );
        }

        void downTool_oncomplete(int AllCompleteCount)
        {
            isrun = false;
            ButtonSS.Dispatcher.BeginInvoke(new Action(() =>
            {
                ButtonSS.Content = "开始";
                if (isotherRun)
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show(this, "完成下载：" + AllCompleteCount + " 错误:" + downTool.errdm.Count);


                }
            }));


        }

        private void ComboBoxZoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int z = (int)e.AddedItems[0];
            Zoom zm = zoomlevel.GetLevel(z, MapType.pm);
            TextBoxX2.Text = zm.maxX.ToString();
            TextBoxY2.Text = zm.maxX.ToString();
            TextBoxAll.Text = ((zm.maxX + 1) * (zm.maxY + 1)).ToString();
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
            TextBoxAll.Text = (startnum - endnum).ToString();
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

        private void ButtonStartTasks_Click(object sender, RoutedEventArgs e)
        {

            int startx = 0, endx = 0, starty = 0, endy = 0, zoom = 0, t = 1, sx = 0, sy = 0,ex=0,ey=0,tasknum=0;

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


            List<Process> processes = new List<Process>();
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
                                + " " + zoom + " " + maptypeindex);
                            p.StartInfo = psi;
                            
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
        }
    }
}
