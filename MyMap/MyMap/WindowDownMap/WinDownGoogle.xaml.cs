using MyMap.ToolHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyMap.WindowDownMap
{
    /// <summary>
    /// DownGoogle.xaml 的交互逻辑
    /// </summary>
    public partial class WinDownGoogle : Window
    { private bool isrun = false;
        private DownTool downTool;
        public WinDownGoogle()
        {
            InitializeComponent();
            downTool = new DownTool();
            for (int i = 4; i < 21; i++)
            {
                ComboBoxZoom.Items.Add(i);
            }
            ComboBoxZoom.SelectedIndex = 0;
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

                
                downTool.oncomplete += downTool_oncomplete;
                downTool.onprecess += downTool_onprecess;

                int x1 = 0, x2 = 0, y1 = 0, y2 = 0, zoom = 0;

                int.TryParse(TextBoxX1.Text, out x1);
                int.TryParse(TextBoxX2.Text, out x2);
                int.TryParse(TextBoxY1.Text, out y1);
                int.TryParse(TextBoxY2.Text, out y2);
                zoom = int.Parse(ComboBoxZoom.SelectedItem.ToString());

                downTool.DownStart(x1, x2, y1, y2, zoom, "d:/googlepic", 5);

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
            TextBoxcompletecount.Dispatcher.BeginInvoke( new Action(
                ()=>TextBoxcompletecount.Text=value.ToString()
                )
                ,null
            );
        }

        void downTool_oncomplete(int AllCompleteCount)
        {
            isrun = false;
            ButtonSS.Content = "开始";
            MessageBox.Show("完成下载：" + AllCompleteCount + " 错误:" + downTool.errdm.Count);
             
        }

        private void ComboBoxZoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int z = (int) e.AddedItems[0];
          Zoom zm=  zoomlevel.GetLevel(z);
            TextBoxX2.Text = zm.maxX.ToString();
            TextBoxY2.Text = zm.maxX.ToString();
            TextBoxAll.Text = ((zm.maxX+1)*(zm.maxY+1)).ToString();
        }

    }
}
