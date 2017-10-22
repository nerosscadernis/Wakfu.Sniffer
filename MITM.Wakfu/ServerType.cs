using System;
using System.Collections.Generic;
using System.Text;

namespace MITM.Wakfu
{
    public class ServerType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Commu { get; set; }
        public string Ip { get; set; }
        public List<int> Ports { get; set; }
        public byte Position { get; set; }
    }
}
