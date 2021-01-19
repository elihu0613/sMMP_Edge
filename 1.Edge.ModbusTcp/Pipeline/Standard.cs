using Edge.ModbusTcp.Components;
using Edge.ModbusTcp.Models;
using Lib.Common.Components.Agreements;
using Lib.Common.Components.Models;
using Lib.Common.Manager;
using Lib.Common.Manager.Models;
using Microsoft.Extensions.Configuration;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static Lib.Common.Manager.GlobalVariables;

namespace Edge.ModbusTcp.Pipeline
{
    internal class Standard : ProtocolFactory
    {
        public override async Task SendAsync(IConfigurationRoot root)
        {
            ModbusTcpManager.RebootModbusTcp = false;

            int iFrequency = 0;

            YamlBase.Modules.Where(c => c.Launcher == nameof(Communication.ModbusTcp)).Select(c => new
            {
                c.Frequency

            }).ToList().ForEach(c =>
            {
                iFrequency = c.Frequency;
            });

            List<MachineShell> MachineBoxes = new();

            root.GetSection(nameof(ModbusTcpTitle.MachineBox)).GetChildren().Select(c => new
            {
                machineNo = c.GetValue<string>(nameof(ModbusTcpRoot.MachineNo)),
                production = c.GetValue<bool>(nameof(ModbusTcpRoot.Production)),
                disabled = c.GetValue<bool>(nameof(ModbusTcpRoot.Disabled)),
                vesion = c.GetValue<string>(nameof(ModbusTcpRoot.Version)),
                address = c.GetValue<string>(nameof(ModbusTcpRoot.Address)),
                port = c.GetValue<int>(nameof(ModbusTcpRoot.Port)),
                map = c.GetSection(nameof(ModbusTcpRoot.Map)).GetChildren().Select(c => new
                {
                    disabled = c.GetValue<bool>(nameof(ElementBox.Disabled)),
                    channel = c.GetValue<string>(nameof(ElementBox.Channel)),
                    functionCode = c.GetValue<int>(nameof(ElementBox.FunctionCode)),
                    slaveAddress = c.GetValue<byte>(nameof(ElementBox.SlaveAddress)),
                    startAddress = c.GetValue<ushort>(nameof(ElementBox.StartAddress)),
                    Points = c.GetSection(nameof(ElementBox.NumberOfPoints)).GetChildren().Select(c => new
                    {
                        pointNo = c.GetValue<int>(nameof(Numberofpoint.PointNo)),
                        attribName = c.GetValue<string>(nameof(Numberofpoint.AttribName))

                    }).ToList()

                }).ToList()

            }).ToList().ForEach(c =>
            {
                if (c.disabled) return;

                List<MessageBox> boxes = new();

                c.map.ForEach(c =>
                {
                    if (c.disabled) return;

                    HostChannel Channel = c.channel switch
                    {
                        nameof(HostChannel.Status) => HostChannel.Status,
                        nameof(HostChannel.Parameter) => HostChannel.Parameter,
                        nameof(HostChannel.Production) => HostChannel.Production,
                        _ => HostChannel.Undefined
                    };

                    if (Channel == HostChannel.Undefined) return;

                    List<PickPoint> points = new();

                    c.Points.ForEach(c =>
                    {
                        points.Add(new()
                        {
                            PointNo = c.pointNo,
                            AttribName = c.attribName
                        });
                    });

                    boxes.Add(new()
                    {
                        Channel = Channel,
                        FunctionCode = c.functionCode,
                        SlaveAddress = c.slaveAddress,
                        StartAddress = c.startAddress,
                        PickPoints = points
                    });
                });

                MachineBoxes.Add(new()
                {
                    MachineNo = c.machineNo,
                    Production = c.production,
                    Vesion = c.vesion,
                    Address = c.address,
                    Port = c.port,
                    MessageBoxes = boxes
                });
            });

            void Working(object obj)
            {
                MachineBoxes.ForEach(async c =>
                {
                    await Task.Run(() =>
                    {
                        int iPort = c.Port;
                        string sMachineNo = c.MachineNo, sAddress = c.Address;

                        lock (MachineSwitch) if (!MachineSwitch.ContainsKey(sMachineNo)) MachineSwitch.Add(sMachineNo, true);

                        c.MessageBoxes.ForEach(async c =>
                        {
                            try
                            {
                                if (c.PickPoints.Count == 0) return;

                                using TcpClient client = new TcpClient(sAddress, iPort);
                                using ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                                ushort[] result = await master.ReadHoldingRegistersAsync(c.SlaveAddress, c.StartAddress, Convert.ToUInt16(c.PickPoints.Count));

                                if (result == null || result.Length == 0) return;

                                MachineSwitch[sMachineNo] = true;

                                c.PickPoints.ForEach(c =>
                                {
                                    string sKey = sMachineNo + "#" + c.AttribName;

                                    lock (RowBox)
                                    {
                                        if (!RowBox.ContainsKey(sKey))
                                        {
                                            RowBox.Add(sKey, result[c.PointNo]);
                                        }
                                        else
                                        {
                                            RowBox[sKey] = result[c.PointNo];
                                        }
                                    }
                                });
                            }
                            catch (Exception e)
                            {
                                if (MachineSwitch[sMachineNo] == true)
                                {
                                    //webapi <==

                                    Console.WriteLine($" No.{sMachineNo} => {e.Message}");
                                }

                                MachineSwitch[sMachineNo] = false;
                            }
                        });
                    });
                });
            }

            await Task.Run(() =>
            {
                try
                {
                    Callback += new TimerCallback(Working);
                    Callback += new TimerCallback(RocketLaunch);
                    Timer = new Timer(Callback, null, Timeout.Infinite, iFrequency);
                    Timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(iFrequency));
                }
                catch (Exception e)
                {
                    Console.WriteLine("sMMP => " + e.Message + "\n" + e.StackTrace);
                }
            });

            void RocketLaunch(object obj)
            {
                MachineBoxes.ToList().ForEach(async c =>
                {
                    await Task.Run(() =>
                    {
                        bool bProduction = c.Production;
                        string sMachineNo = c.MachineNo, sVersion = c.Vesion;

                        List<Parameter> parameters = new();

                        c.MessageBoxes.ForEach(c =>
                        {
                            c.PickPoints.ForEach(c =>
                            {
                                string sKey = sMachineNo + "#" + c.AttribName;

                                if (RowBox.ContainsKey(sKey))
                                {
                                    lock (HistoryBox)
                                    {
                                        if (!HistoryBox.ContainsKey(sKey))
                                        {
                                            HistoryBox.Add(sKey, RowBox[sKey]);
                                        }
                                        else
                                        {
                                            if (RowBox[sKey] != HistoryBox[sKey])
                                            {
                                                parameters.Add(new()
                                                {
                                                    AttribNo = c.AttribName,
                                                    AttribValue = RowBox[sKey]
                                                });

                                                HistoryBox[sKey] = RowBox[sKey];
                                            }
                                        }
                                    }
                                }
                            });

                            GlobalVariables globally = new();

                            GlobalApproach.PushDataToHost(c.Channel, new()
                            {
                                Version = sVersion,
                                Production = bProduction,
                                MachineNo = sMachineNo,
                                ReportDateTime = globally.NowTime,
                                Row = parameters
                            });
                        });
                    });
                });
            }
        }

        private static Timer Timer { get; set; }
        private static TimerCallback Callback { get; set; }

        private static readonly Dictionary<string, bool> MachineSwitch = new();

        private static readonly Dictionary<string, ushort> HistoryBox = new();

        private static readonly Dictionary<string, ushort> RowBox = new();
    }
}