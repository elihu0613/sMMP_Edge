using Lib.Common.Manager;
using Lib.Common.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Lib.Common.Components.Verification
{
    public static class FormattingXML
    {
        public static FoundationRoot FoundationFilter(this string value)
        {
            FoundationRoot result = new();
            XNamespace ns = GlobalVariables.Foundation;

            //GlobalApproach approach = new();

            //if (value == "" || value == null)
            //{
            //    //lock (approach) GlobalApproach.LocalBuilder(new FoundationWriter(), new(new byte[] { 0, 0, 0, 0 }), null);

            //    if (GlobalApproach.LocalBuilder(new FoundationWriter(), new(new byte[] { 0, 0, 0, 0 }), null)) return null;
            //}

            try
            {
                XDocument xDocument = XDocument.Parse(value);
                result.Server.Name = xDocument.Root.Element(ns + "Server").Attribute("Name").Value;
                result.Disabled = xDocument.Root.Element(ns + "Server").Attribute("Disabled").Value == "1";
                result.Server.Address = xDocument.Root.Element(ns + "Server").Element(ns + "URL").Value;

                result.Eai.Name = xDocument.Root.Element(ns + "Server").Element(ns + "Eai").Attribute("Name").Value;
                result.Eai.Version = xDocument.Root.Element(ns + "Server").Element(ns + "Eai").Attribute("Version").Value;
                result.Eai.Account = xDocument.Root.Element(ns + "Server").Element(ns + "Eai").Attribute("Account").Value;
                result.Eai.URL = xDocument.Root.Element(ns + "Server").Element(ns + "Eai").Element(ns + "URL").Value;

                result.Edge.Name = xDocument.Root.Element(ns + "Edge").Attribute("Name").Value;
                result.Edge.Version = xDocument.Root.Element(ns + "Edge").Attribute("Version").Value;

                result.Server.MqttPort = int.Parse(xDocument.Root.Element(ns + "Server").Element(ns + "MqttPort").Value);

                List<ModbusTcpRoot> modbusTcpRoots = new();

                xDocument.Root.Descendants(ns + "ModbusTcp").Elements(ns + "Machine").Select(c => new
                {
                    machineNo = c.Attribute("Name").Value,
                    production = c.Attribute("Production").Value == "1",
                    disabled = c.Attribute("Disabled").Value == "1",
                    version = c.Element(ns + "Version").Value,
                    address = c.Element(ns + "Address").Value,
                    port = int.Parse(c.Element(ns + "Port").Value),
                    parameter = c.Descendants(ns + "Row").Elements(ns + "Parameter").Select(c => new
                    {
                        disabled = c.Attribute("Disabled").Value == "1",
                        channel = c.Attribute("Channel").Value,
                        functionCode = int.Parse(c.Attribute("FunctionCode").Value),
                        slaveAddress = byte.Parse(c.Attribute("SlaveAddress").Value),
                        startAddress = ushort.Parse(c.Attribute("StartAddress").Value),
                        element = c.Elements(ns + "Element").Select(c => new
                        {
                            id = int.Parse(c.Attribute("Id").Value),
                            attribName = c.Attribute("AttribName").Value

                        }).ToList()

                    }).ToList(),

                }).ToList().ForEach(c =>
                {
                    List<ElementBox> elementBoxes = new();

                    c.parameter.ForEach(c =>
                    {
                        List<Numberofpoint> numberofpoints = new();

                        c.element.ForEach(c =>
                        {
                            numberofpoints.Add(new()
                            {
                                PointNo = c.id,
                                AttribName = c.attribName
                            });
                        });

                        elementBoxes.Add(new()
                        {
                            Disabled = c.disabled,
                            Channel = c.channel,
                            FunctionCode = c.functionCode,
                            SlaveAddress = c.slaveAddress,
                            StartAddress = c.startAddress,
                            NumberOfPoints = numberofpoints
                        });
                    });

                    modbusTcpRoots.Add(new()
                    {
                        MachineNo = c.machineNo,
                        Production = c.production,
                        Disabled = c.disabled,
                        Version = c.version,
                        Address = c.address,
                        Port = c.port,
                        Map = elementBoxes
                    });
                });

                result.ModbusTCP = modbusTcpRoots;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Foundation Filter XML => " + e.Message + "\n" + e.StackTrace);
            }

            return result;
        }
    }
}