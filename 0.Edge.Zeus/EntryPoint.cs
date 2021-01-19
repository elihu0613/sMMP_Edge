using Edge.Zeus.Panels;
using Lib.Common.Components.Agreements;
using Lib.Common.Manager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using static Lib.Common.Manager.GlobalVariables;

namespace Edge.Zeus
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            Console.Title = EdgeTitle + EdgeVersion;

            if (Directory.Exists(IIOTFilePath))
            {
                DInfo = new(IIOTFilePath);
                DInfo.Delete(true);
            }

            DInfo = new DirectoryInfo(LocalFilePath).CreateSubdirectory(YamlBase.Protagonist.IIOTFileName);

            if (args.Length == 0 || args[0] != YamlBase.Protagonist.LocalName) return;

            FoundationProvider.ReadDocument();

            PipeBuilder.BuildAsync();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Runon>();

                GlobalApproach.PipeBuilder(true, Communication.EdgeService);

                EnableEdgeService = true;
            });

        private static DirectoryInfo DInfo { get; set; }
    }
}