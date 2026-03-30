using QLocalPing.Helper;
using QLocalPing.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace QLocalPing.ViewModels
{
    public class OnlineIpListPageViewModel : INotifyPropertyChanged
    {
        private Dictionary<NetworkGroupModel, NetworkGroupViewModel> ModelViewModelRelation = new();
        public ObservableCollection<NetworkGroupViewModel> NetworkGroups { get; private set; } = new ObservableCollection<NetworkGroupViewModel>();

        public event PropertyChangedEventHandler? PropertyChanged;
        public NetworkTargetModel SelectedNetworkTargetViewModel { get; set; }
        public ICommand RefrushCommand { get; private set; }
        public ICommand CopyAddressCommand { get; private set; }
        private bool _IsNowRefrushing = false;
        public bool IsNowRefrushing
        {
            get => _IsNowRefrushing; set
            {
                _IsNowRefrushing = value;
                PropertyChanged?.Invoke(this, new("IsNowRefrushing"));
            }
        }

        private bool _IsNowCopyAddressing = false;
        public bool IsIsNowCopyAddressing
        {
            get => _IsNowCopyAddressing; set
            {
                _IsNowCopyAddressing = value;
                PropertyChanged?.Invoke(this, new("IsIsNowCopyAddressing"));
            }
        }


        public OnlineIpListPageViewModel()
        {
            RefrushCommand = new ActionCommand(async o =>
            {
                if (o is NetworkGroupModel md)
                {
                    await RefrushNetworkGroup(md);
                }
            }); CopyAddressCommand = new ActionCommand(async o =>
            {
                if (SelectedNetworkTargetViewModel != null)
                {
                    Clipboard.SetText(SelectedNetworkTargetViewModel.IPAddress.ToString());
                }
                if (!IsIsNowCopyAddressing)
                {
                    IsIsNowCopyAddressing = true;
                    _=Task.Run(() =>
                    {
                        Thread.Sleep(1500);
                        App.Current.Dispatcher.BeginInvoke(() =>
                        {
                            IsIsNowCopyAddressing = false;
                        });
                    });
                }
            });
        }

        public async Task StartFindTargets(Func<bool> CanConnection)
        {
            IsNowRefrushing = true;
            await NetworkHelp.FindNetworkTargetAndGroups(group =>
            {
                App.Current.Dispatcher.BeginInvoke(() =>
                {
                    NetworkGroupViewModel vm = new NetworkGroupViewModel(group);
                    NetworkGroups.Add(vm);
                    ModelViewModelRelation.Add(group, vm);
                    vm.Parent = this;
                });
            }, (group, target) =>
            {
                App.Current.Dispatcher.BeginInvoke(() =>
                {
                    ModelViewModelRelation[group].AddOnlineTargets(target);
                });
            }, CanConnection);
            IsNowRefrushing = false;
        }
        public async Task RefrushNetworkGroup(NetworkGroupModel Model)
        {
            if (IsNowRefrushing) return;
            IsNowRefrushing = true;
            var viewModel = ModelViewModelRelation[Model];
            viewModel.OnlineTarges.Clear();
            await NetworkHelp.FindNetworkGroupTarget(Model, (group, target) =>
            {
                App.Current.Dispatcher.BeginInvoke(() =>
                {
                    viewModel.AddOnlineTargets(target);
                });
            });
            IsNowRefrushing = false;
        }

    }
}
