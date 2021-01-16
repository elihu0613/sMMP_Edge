using System.Collections.Generic;

namespace Lib.Common.Manager.Models
{
    public class FoundationRoot
    {
        public string UpdateTime { get; set; }
        public bool Disabled { get; set; }
        public ServerMap Server { get; set; } = new();
        public EdgeMap Edge { get; set; } = new();
        public EaiMap Eai { get; set; } = new();

        public List<ModbusTcpRoot> ModbusTCP = new();

        //public List<OpcUa> OpcUa = new();

        //public List<WebApi> WebApi = new();
    }

    public class ServerMap
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int MqttPort { get; set; }
    }

    public class EdgeMap
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class EaiMap
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Account { get; set; }
        public string URL { get; set; }
    }
}
