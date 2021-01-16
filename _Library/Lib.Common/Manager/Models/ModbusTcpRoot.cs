﻿using System.Collections.Generic;

namespace Lib.Common.Manager.Models
{
    public class ModbusTcpRoot
    {
        public string MachineNo { get; set; }
        public bool Production { get; set; }
        public bool Disabled { get; set; }
        public string Version { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public List<ElementBox> Map { get; set; }
    }

    public class ElementBox
    {
        public bool Disabled { get; set; }
        public string Channel { get; set; }
        public int FunctionCode { get; set; }
        public byte SlaveAddress { get; set; }
        public ushort StartAddress { get; set; }
        public List<Numberofpoint> NumberOfPoints { get; set; }
    }

    public class Numberofpoint
    {
        public int PointNo { get; set; }
        public string AttribName { get; set; }
    }

    public class ModbusTcpTitle
    {
        public List<ModbusTcpRoot> MachineBox { get; set; }
    }
}