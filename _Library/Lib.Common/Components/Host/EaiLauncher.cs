using Lib.Common.Components.Agreements;
using Lib.Common.Components.Models;
using Lib.Common.Manager;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lib.Common.Components.Func;
using static Lib.Common.Components.Func.ExtensionTools;

namespace Lib.Common.Components.Host
{
    public class EaiLauncher : BaseStationFactory
    {
        public override async Task SendAsync(PayloadRoot root, HostChannel channel)
        {
            try
            {
                if (channel == HostChannel.Undefined || root.Row.Count == 0) return;

                FoundationProvider provider = new();
                GlobalVariables variables = new();

                XElement xHost = new("host");
                xHost.Add(new XAttribute("prod", provider.FoundationBasic.Edge.Name));
                xHost.Add(new XAttribute("ver", provider.FoundationBasic.Edge.Version));
                xHost.Add(new XAttribute("ip", GeneralTools.GetLocalIP()));
                xHost.Add(new XAttribute("acct", provider.FoundationBasic.Eai.Account));
                xHost.Add(new XAttribute("timestamp", variables.NowTime));

                XElement service = new("service");
                service.Add(new XAttribute("prod", provider.FoundationBasic.Eai.Name));
                service.Add(new XAttribute("name", channel switch
                {
                    HostChannel.Status => "change.machine.status.process",
                    HostChannel.Parameter => "parameter.check.process",
                    HostChannel.Production => "production.edc.process",
                    _ => ""
                }));

                XElement xRoot = new XElement("request");
                xRoot.Add(new XAttribute("key", (xHost.ToString() + service.ToString()).ToMD5()));
                xRoot.Add(new XAttribute("type", "sync"));
                xRoot.Add(xHost);
                xRoot.Add(service);

                XElement xPayload = new("payload");

                XElement xParam = new("param");
                xParam.Add(new XAttribute("key", "std_data"));
                xParam.Add(new XAttribute("type", "xml"));
                xPayload.Add(xParam);

                XElement xDataRequest = new("data_request");
                xParam.Add(xDataRequest);

                XElement xDatainfo = new("datainfo");
                xDataRequest.Add(xDatainfo);

                XElement xParameter = new("parameter");
                xParameter.Add(new XAttribute("key", channel switch
                {
                    HostChannel.Status => "change_machine_status",
                    HostChannel.Parameter => "parameter_check",
                    HostChannel.Production => "production_edc",
                    _ => ""

                }));
                xParameter.Add(new XAttribute("type", "data"));

                xDatainfo.Add(xParameter);

                XElement xData = new("data");
                xData.Add(new XAttribute("name", channel switch
                {
                    HostChannel.Status => "change_machine_status",
                    HostChannel.Parameter => "parameter_check",
                    HostChannel.Production => "production_edc",
                    _ => ""

                }));

                xParameter.Add(xData);

                XElement xRow = new("row");
                xRow.Add(new XAttribute("seq", ""));

                XElement xField = new("field", root.MachineNo);
                xField.Add(new XAttribute("name", "machine_no"));
                xField.Add(new XAttribute("type", "string"));

                root.Row.ForEach(c =>
                {
                    bool bStatus = false;

                    XElement field = new("field");

                    switch (channel)
                    {
                        case HostChannel.Status:
                            if (c.AttribNo == nameof(HostChannel.Status).FormatFirstCapitalized())
                            {
                                bStatus = true;
                                field.Add(new XAttribute("name", "machine_status"));
                                field.Add(new XAttribute("type", "numeric"));
                                field.Add(c.AttribValue);
                            }
                            break;

                        case HostChannel.Parameter:
                            break;
                    };

                    xField.Add(field);

                    //if (bStatus)
                    //{
                    //    xRow.Add(new XElement("field", variables.NowTime, new XAttribute("name", "report_datetime"), new XAttribute("type", "date")));
                    //}
                });

                xRow.Add(xField);

                xRow.Add(new XElement("field", variables.NowTime, new XAttribute("name", "report_datetime"), new XAttribute("type", "date")));

                xData.Add(xRow);

                xRoot.Add(xPayload);

                var request = new EaiServiceSOAP.IntegrationEntryClient(EaiServiceSOAP.IntegrationEntryClient.EndpointConfiguration.IntegrationEntry, provider.FoundationBasic.Eai.URL);
                var response = await request.invokeSrvAsync(xRoot.ToString());
                string result = response.Body.invokeSrvReturn;

                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ModbusTCP: {e.Message}\n{e.StackTrace} ");
            }
        }
    }
}