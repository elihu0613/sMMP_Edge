using Lib.Common.Components.Agreements;
using System;
using System.IO;
using System.Xml;

namespace Edge.Zeus.Controllers
{
    public class WebServiceSOAP : ISoapServer
    {
        public string invokeSrv(string inXML)
        {
            string result = null;

            //try
            //{
            //    StringWriter stringWriter = new StringWriter();
            //    XmlDocument xmlDocument = new XmlDocument();
            //    xmlDocument.LoadXml(inXML);
            //    xmlDocument.Save(stringWriter);

            //    //if (EQXHelper.IsDebug) _logger.LogWarning(stringWriter.ToString());
            //    stringWriter.Dispose();
            //    stringWriter.Close();

            //    Module += (Message.GetSingleAttribute("request/host", "prod", xmlDocument)) switch
            //    {
            //        "sMES" => Message.RespSMES,
            //        "SCADA" => Message.RespScada,
            //        "sMES_Batom" => Message_Batom.RespSMES,

            //        _ => throw new Exception("sMMP:Request system name format error")
            //    };

            //    result = Module(xmlDocument);
            //}
            //catch (Exception e)
            //{
            //    result = e.Message;

            //    //_logger.LogError($"@@@@@@@@@@@\n{result} ");
            //}

            return result;
        }

        private Func<XmlDocument, string> Module { get; set; }
    }
}
