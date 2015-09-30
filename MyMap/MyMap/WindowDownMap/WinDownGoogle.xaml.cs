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
    {
        public WinDownGoogle()
        {
            InitializeComponent();
        }

        private bool isrun = false;
        private void ButtonSS_Click(object sender, RoutedEventArgs e)
        {
            if (isrun)
            {
                isrun = false;
                ButtonSS.Content = "开始";
            }
            else
            {

                DownTool downTool=new DownTool();
             List<string> urls=  downTool.DownStart(0,15,0,15);

                isrun = true;
                ButtonSS.Content = "停止";
            }
        }
    }
}
