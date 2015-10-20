using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Forms;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace ToMyHeart
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// 选中边框
        /// </summary>
        private Brush selectBrush;
        /// <summary>
        /// 未选中边框
        /// </summary>
        private Brush noSelectBrush;

        private string imagepath = "";

        private bool mouseDown = false;

        public int mouseDownX { get; set; }
        public int mouseDownY { get; set; }

        private Point mouseDownPoint;
        private Point mouseDownPointByBorder;

        private Border selectElement;
        public MainWindow()
        {
            InitializeComponent();
            selectBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            noSelectBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));

        }



        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            //添加元素

            Border textBorder = new Border();
            textBorder.BorderThickness = new Thickness(1);
            textBorder.BorderBrush = noSelectBrush;


            TextBlock textBlock = new TextBlock();
            textBorder.Child = textBlock;
            textBlock.Text = ("xxxxxxx" + (CanvasDraw.Children.Count + 1)).PadRight(8);
            textBorder.Tag = new MyUITextBlock() { mx = 0, my = 0, FontFamily = textBlock.FontFamily };
            var fonts = Fonts.SystemFontFamilies;

            CanvasDraw.Children.Add(textBorder);


        }

        private void CanvasDraw_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;

            var textblock = e.Source as TextBlock;
            if (textblock != null)
            {
                var border = textblock.Parent as Border;
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = selectBrush;
                selectElement = border;

                mouseDownPoint = e.GetPosition(sender as Canvas);
                mouseDownPointByBorder = e.GetPosition(border);
            }
            else
            {
                selectElement = null;
                var canvas = sender as Canvas;
                foreach (var item in canvas.Children)
                {

                    if (item.GetType().FullName == "System.Windows.Controls.Border")
                    {
                        var border = item as Border;
                        border.BorderThickness = new Thickness(1);
                        border.BorderBrush = noSelectBrush;
                    }
                }
            }
        }

        private void CanvasDraw_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
            if (selectElement != null)
            {
                MyUITextBlock myui = selectElement.Tag as MyUITextBlock;
                myui.mx = Canvas.GetLeft(selectElement);
                myui.my = Canvas.GetTop(selectElement);

            }



        }

        private void CanvasDraw_OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point mouseMovepPoint = e.GetPosition(sender as Canvas);
            Vector move = Point.Subtract(mouseMovepPoint, mouseDownPoint);

            if (mouseDown)
            {

                if (selectElement != null)
                {
                    MyUITextBlock myUi = selectElement.Tag as MyUITextBlock;
                    if (myUi != null)
                    {
                        Canvas.SetLeft(selectElement, move.X + myUi.mx);
                        Canvas.SetTop(selectElement, move.Y + myUi.my);

                    }
                }
            }
        }

        private void ButtonFont_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fd = new FontDialog();
            FontFamily fontFamily = fd.ShowDialog();

            if (fontFamily != null && selectElement != null)
            {
                MyUITextBlock myUi = selectElement.Tag as MyUITextBlock;
                myUi.FontFamily = fontFamily;

                TextBlock textBlock = selectElement.Child as TextBlock;
                textBlock.FontFamily = fontFamily;
            }

        }

        private void TextBoxSize_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string sizestr = (e.Source as System.Windows.Controls.TextBox).Text;
            int size = 12;
            if (int.TryParse(sizestr, out size)
                && selectElement != null
                && size > 0)
            {
                TextBlock textBlock = selectElement.Child as TextBlock;
                textBlock.FontSize = size;
            }
        }

        private void ButtonLoad_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "*.jpg|*.jpg";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                imagepath = openFileDialog.FileName;
                BitmapImage bitmapImage = new BitmapImage(new Uri(imagepath));
                if (bitmapImage.SourceRect.X > 1000
                    || bitmapImage.SourceRect.Y > 1000)
                {
                    System.Windows.MessageBox.Show("图片大小1000X1000以下");
                }
                else
                {
                    ImageBack.Source = bitmapImage;
                }
            }
        }

        private void ButtonColor_OnClick(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (selectElement != null)
                {
                    MyUITextBlock myUi = selectElement.Tag as MyUITextBlock;
                    myUi.FontColor = Color.FromRgb(colorDialog.Color.R,
                        colorDialog.Color.B,
                        colorDialog.Color.G);
                    TextBlock textBlock = selectElement.Child as TextBlock;
                    textBlock.Foreground = new SolidColorBrush(myUi.FontColor);
                }

            }
        }

        private void ButtonBuild_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CanvasDraw.Children.Count > 0)
                {

                    Bitmap bitmap = new Bitmap((int)ImageBack.Source.Width, (int)ImageBack.Source.Height);

                    System.Drawing.Image img = System.Drawing.Image.FromFile(imagepath);
                    Graphics gra = Graphics.FromImage(bitmap);
                    gra.DrawImage(img, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));
                    foreach (var item in CanvasDraw.Children)
                    {
                        if (item.GetType().FullName == "System.Windows.Controls.Border")
                        {
                            Border b = item as Border;
                            MyUITextBlock myUiTextBlock = b.Tag as MyUITextBlock;
                            TextBlock tb = b.Child as TextBlock;

                            gra.DrawString(tb.Text
                                , new Font(myUiTextBlock.FontFamily.Source, (float)tb.FontSize),
                                new SolidBrush(System.Drawing.Color.FromArgb(myUiTextBlock.FontColor.R,
                                    myUiTextBlock.FontColor.G, myUiTextBlock.FontColor.B)),
                                    new RectangleF((float)myUiTextBlock.mx, (float)myUiTextBlock.my, (float)tb.Width, (float)tb.Height)
                               );
                        }
                    }

                    bitmap.Save("d:/ff.jpg", ImageFormat.Jpeg);
                }
                else
                {
                    System.Windows.MessageBox.Show("别逗我，啥也没有怎么弄啊？");
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }

        }
    }
}
