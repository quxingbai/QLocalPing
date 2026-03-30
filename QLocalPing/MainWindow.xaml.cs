using System.ComponentModel;
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

namespace QLocalPing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point? MouseDownPoint = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        #region View

        private void BT_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BD_DragMove_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void BD_DragMove_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var nowPoint = e.GetPosition(this);
            int MouseMoveActionDistance = 20;
            if (Mouse.LeftButton == MouseButtonState.Pressed && MouseDownPoint != null && WindowState == WindowState.Maximized && PointDistance(nowPoint, MouseDownPoint.Value) > MouseMoveActionDistance)
            {
                WindowState = WindowState.Normal;
                var w = RenderSize.Width;
                var h = RenderSize.Height;
                var left = nowPoint.X - w / 2;
                var top = -10;
                this.Left = left;
                this.Top = top;
                DragMove();
            }
        }
        private void BD_DragMove_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseDownPoint = e.GetPosition(this);
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void BD_DragMove_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseDownPoint = null;
        }
        public static double PointDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        #endregion
    }
}