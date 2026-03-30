using QLocalPing.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace QLocalPing.ViewModels
{

    public class NetworkGroupViewModel : INotifyPropertyChanged
    {

        public OnlineIpListPageViewModel Parent { get; set; }
        public NetworkGroupModel NetworkGroup { get; set; }
        public ObservableCollection<NetworkTargetModel> OnlineTarges { get; set; } = new ObservableCollection<NetworkTargetModel>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public string GroupName => NetworkGroup.SubertIp;

        public NetworkGroupViewModel(NetworkGroupModel networkGroup)
        {
            NetworkGroup = networkGroup;
        }
        public void AddOnlineTargets(NetworkTargetModel Item)
        {
            if (OnlineTarges.Any(a=>a.Equals(Item))) return;
            OnlineTarges.Add(Item);
        }
    }
}
