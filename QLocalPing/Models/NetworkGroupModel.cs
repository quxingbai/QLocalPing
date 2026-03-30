using QLocalPing.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace QLocalPing.Models
{
    public class NetworkGroupModel
    {
        public string SubertIp {  get; set; }
        public IEnumerable<NetworkTargetModel> Targets { get; set; }
    }
}
