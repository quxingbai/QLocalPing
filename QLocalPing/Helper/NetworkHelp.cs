using QLocalPing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace QLocalPing.Helper
{
    public static class NetworkHelp
    {
        private static object _lock = new object();
        public static List<IPAddress> GetLocalIPAddresses()
        {
            var ipList = new List<IPAddress>();
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // IPv4
                {
                    ipList.Add(ip);
                }
            }

            return ipList;
        }
        public static bool HasIpv4 => GetLocalIPAddresses().Any();
        public async static Task<IEnumerable<NetworkGroupModel>> GetNetworkTargetGroupsAsync()
        {
            List<NetworkGroupModel> groups = new();
            await Task.Run(() =>
            {
                foreach (var i in GetLocalIPAddresses())
                {
                    string ip = GetIpSubentAddress(i);
                    groups.Add(new() { SubertIp = ip });
                }
            });
            return groups;
        }

        /// <summary>
        /// 获取一个IP的前三位String
        /// </summary>
        private static string GetIpSubentAddress(IPAddress Ip)
        {
            var ipstr = Ip.ToString();
            string ip = ipstr.Substring(0, ipstr.LastIndexOf('.'));
            return ip;
        }


        public async static Task FindNetworkTargetAndGroups(Action<NetworkGroupModel> FindedGroup, Action<NetworkGroupModel, NetworkTargetModel> FindedGroupTarget, Func<bool> IsConnectionFind = null, int ThreadCount = 50)
        {
            List<NetworkGroupModel> groups = new();
            await Task.Run(async () =>
            {
                foreach (var g in await GetNetworkTargetGroupsAsync())
                {
                    FindedGroup(g);
                    groups.Add(g);
                    await FindNetworkGroupTarget(g, FindedGroupTarget, IsConnectionFind, ThreadCount);
                }
            });
        }
        /// <summary>
        /// 通过ip组获取所有可ping通的ip地址
        /// </summary>
        /// <param name="GroupSource">组</param>
        /// <param name="FindedGroupTarget">找到了新的地址</param>
        /// <param name="IsConnectionFind">是否可以继续找</param>
        /// <param name="ThreadCount">线程数量</param>
        public async static Task FindNetworkGroupTarget(NetworkGroupModel GroupSource, Action<NetworkGroupModel, NetworkTargetModel> FindedGroupTarget, Func<bool> IsConnectionFind = null, int ThreadCount = 50)
        {
            IsConnectionFind = (IsConnectionFind ?? new Func<bool>(() => true));

            await Task.Run(async () =>
            {
                List<NetworkTargetModel> targets = new();
                GroupSource.Targets = targets;
                await FindGroupItems(GroupSource, target =>
                {
                    targets.Add(target);
                    FindedGroupTarget(GroupSource, target);
                });
            });

            async Task FindGroupItems(NetworkGroupModel group, Action<NetworkTargetModel> Finded, int TimeOut = 1250)
            {
                await Task.Run(() =>
                {
                    int MaxThreadCount = ThreadCount;
                    int NowThreadCount = 0;
                    Queue<int> Tasks = new();
                    for (int i = 1; i <= 254; i++)
                    {
                        Tasks.Enqueue(i);
                    }

                    while (IsConnectionFind())
                    {

                        while (NowThreadCount < MaxThreadCount)
                        {
                            bool hasTask = false;
                            lock (_lock)
                            {
                                hasTask = Tasks.Any();
                            }
                            if (hasTask)
                            {
                                NowThreadCount++;
                                ThreadPool.QueueUserWorkItem(w =>
                                {
                                    Execute();
                                    NowThreadCount--;
                                });
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (NowThreadCount == 0 && Tasks.Count == 0)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                    void Execute()
                    {
                        try
                        {
                            Ping ping = new();
                            int task = -1;
                            lock (_lock)
                            {
                                if (Tasks.Any())
                                {
                                    task = Tasks.Dequeue();
                                }
                            }
                            if (task == -1) return;
                            string ip = group.SubertIp + "." + task;
                            var result = ping.Send(ip, TimeOut);
                            if (result.Status == IPStatus.Success)
                            {
                                IPAddress pingIpAddress = IPAddress.Parse(ip);
                                Finded.Invoke(new NetworkTargetModel() { IPAddress = pingIpAddress, PingRoundtripMS = result.RoundtripTime, TTL = result.Options.Ttl });
                            }
                            ping.Dispose();
                            Debug.WriteLine("查找ip" + ip);
                        }
                        catch (Exception err)
                        {
                            Debug.WriteLine("Ping失败\n" + err);
                        }
                    }

                });
            }
        }


    }
}
