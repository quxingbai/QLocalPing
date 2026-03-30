using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace QLocalPing.Models
{
    public class NetworkTargetModel
    {
        public IPAddress IPAddress { get; set; }
        public long PingRoundtripMS {  get; set; }
        public int TTL { get; set; }
        public override bool Equals(object? obj)
        {
            if(obj is NetworkTargetModel model)
            {
                return (model.IPAddress.ToString() == this.IPAddress.ToString()) &&model.TTL==this.TTL&&model.PingRoundtripMS==this.PingRoundtripMS;
            }
            return false;
        }
    }
}
