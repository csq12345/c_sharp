using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CW_MapDown
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

        }

        private int maxstep = 0;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TencentWorker tencentWorker = new TencentWorker();
            maxstep = tencentWorker.StartDown(4, @"d:\mymap");
            tencentWorker.ProgressChanged += tencentWorker_ProgressChanged;
        }

        void tencentWorker_ProgressChanged(int progressPercentage)
        {
            if (progressPercentage == maxstep)
            {
                MessageBox.Show("complete");
            }
        }
    }
}
