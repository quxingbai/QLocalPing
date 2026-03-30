using QLocalPing.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace QLocalPing.Views
{
    /// <summary>
    /// OnlineIpListPage.xaml 的交互逻辑
    /// </summary>
    public partial class OnlineIpListPage : UserControl
    {
        OnlineIpListPageViewModel ViewModel = new();
        public OnlineIpListPage()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            Loaded += OnlineIpListPage_Loaded;
        }

        private void OnlineIpListPage_Loaded(object sender, RoutedEventArgs e)
        {
            _ = ViewModel.StartFindTargets(() => true);
            Loaded -= OnlineIpListPage_Loaded;
        }

    }
}
