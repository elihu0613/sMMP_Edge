﻿using Lib.Common.Manager.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Lib.Common.Manager
{
    public class GlobalVariables
    {
        public static bool EnableCsvFile { get; set; }
        public static bool EnableModbusTcp { get; set; }
        public static bool EnableWebApi { get; set; }
        public static bool EnableOpcUa { get; set; }
        public static bool EnableEdgeService { get; set; }
        public string NowTime { get; } = DateTime.Now.ToString("yyyyMMddHHmmss");
        public static string EaiUrl => BasicMap.FoundationBasic.Server.Address + ":9999/IntegrationEntry";
        public static XNamespace Foundation { get; } = "http://digiwin.com/iiot/foundation";
        private static FoundationProvider BasicMap { get; set; }
        public static string EdgeTitle { get; } = "sMMP_Edge";
        public static string WelcomeTitle { get; } = "D I G I W I N";
        public static string ServiceTitle { get; } = "Server completes startup";
        public static string LocalFileName { get; } = "Configs";

        public static readonly string[] TimeFormat = { "yyyyMMddHHmmss", "yyyyMMddHHmmssfff", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd_HHmmss", "yyyyMMdd_HHmmssfff" };
        public static string EdgeVersion { get; } = " " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public static readonly MainBook YamlBase = MainBuilder.RealMainYml();
        public static string LocalFilePath { get; } = AppDomain.CurrentDomain.BaseDirectory + LocalFileName + "\\";
        public static string IIOTFilePath { get; } = AppDomain.CurrentDomain.BaseDirectory + LocalFileName + "\\" + YamlBase.Protagonist.IIOTFileName + "\\";
        public static string FoundationDocument { get; } = AppDomain.CurrentDomain.BaseDirectory + LocalFileName + "\\" + YamlBase.Protagonist.FoundationDocumentName;
        public static DateTime DtYamlBase { get; set; } = File.GetLastWriteTime(FoundationDocument);

        public GlobalVariables()
        {
            BasicMap = new();
        }
    }
}