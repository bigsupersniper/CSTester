using System;
using System.Collections.Generic;
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

namespace CSTester
{
    /// <summary>
    /// ImageView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageView : Window
    {
        public static bool Opened { get; private set; }

        public ImageView()
        {
            InitializeComponent();
            Opened = true;
        }

        void LoadImage(string url)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.DownloadFailed += (sender, e) =>
            {
                MessageBox.Show(e.ErrorException.ToString(), "图片加载失败", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(url, UriKind.Absolute);
            image.Source = bitmap;
            bitmap.EndInit();
        }

        private void tbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var url = tbUrl.Text.Trim();
                if (url != "")
                {
                    LoadImage(url);
                }
            }
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (image.Source != null)
            {
                if (image.Source is BitmapImage)
                {
                    var bitmap = image.Source as BitmapImage;
                    image.ToolTip = string.Format("宽：{0}\r\n高：{1}\r\nDpiX：{2}\r\nDpiY：{3}", bitmap.PixelWidth,
                        bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Opened = false;
        }
    }
}
