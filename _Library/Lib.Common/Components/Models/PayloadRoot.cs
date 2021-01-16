using System.Collections.Generic;

namespace Lib.Common.Components.Models
{
    public class PayloadRoot
    {
        public string Version { get; set; }
        public bool Production { get; set; }
        public string MachineNo { get; set; }
        public string ReportDateTime { get; set; }
        public List<Parameter> Row { get; set; }
    }

    public class Parameter
    {
        public string AttribNo { get; set; }
        public ushort AttribValue { get; set; }
    }
}
