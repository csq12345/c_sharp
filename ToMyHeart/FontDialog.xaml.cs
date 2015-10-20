using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToMyHeart
{
    /// <summary>
    /// FontDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FontDialog : Window
    {
        private System.Windows.Media.FontFamily fontFamily = null;
        public FontDialog()
        {
            InitializeComponent();
            GetAllFont();
        }

        void GetAllFont()
        {
            XmlLanguage xlcn = XmlLanguage.GetLanguage("zh-cn");
            XmlLanguage xlen = XmlLanguage.GetLanguage("en-us");
            foreach (var item in Fonts.SystemFontFamilies)
            {
                string value = "";

                item.FamilyNames.TryGetValue(xlcn, out value);
                if (value == null)
                {

                    item.FamilyNames.TryGetValue(xlen, out value);
                }
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.DataContext = item;
                listBoxItem.Content = value;
                ListBoxFonts.Items.Add(listBoxItem);
            }


        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            var si = ListBoxFonts.SelectedItem as ListBoxItem;
            if (si != null)
            {
           fontFamily = si.DataContext as FontFamily;

            }
            this.Close();
        }

        public FontFamily ShowDialog()
        {
            base.ShowDialog();
            return fontFamily;
        }
    }
}
