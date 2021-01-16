using Lib.Common.Manager;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using static Lib.Common.Manager.GlobalVariables;

namespace Lib.Common.Components.History
{
    public class LogBuilder
    {
        public static void WriteLog(LogEventLevel eventLevel, string message)
        {
            //從 appsettings.json 讀取設定資料
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            //使用從 appsettings.json 讀取到的內容來設定 logger
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();



            //try
            //{
            //    string divider = new string('-', 100);

            //    string output = eventLevel switch
            //    {
            //        LogEventLevel.Debug => YamlBase.LogFileTemplate + divider,
            //        LogEventLevel.Information => YamlBase.LogFileTemplate + divider,
            //        LogEventLevel.Warning => YamlBase.LogFileTemplate + divider,
            //        LogEventLevel.Error => YamlBase.LogFileTemplate + divider,
            //        LogEventLevel.Fatal => YamlBase.LogFileTemplate + divider,

            //        _ => null
            //    };

            //    Log.Logger = new LoggerConfiguration()
            //        .Enrich.FromLogContext()
            //        //.WriteTo.Console()
            //        .MinimumLevel.Debug()
            //        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Debug).WriteTo.File(LogFilePath(nameof(LogEventLevel.Debug), YamlBase), rollingInterval: RollingInterval.Day, outputTemplate: output))
            //        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.File(LogFilePath(nameof(LogEventLevel.Information), YamlBase), rollingInterval: RollingInterval.Day, outputTemplate: output))
            //        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Warning).WriteTo.File(LogFilePath(nameof(LogEventLevel.Warning), YamlBase), rollingInterval: RollingInterval.Day, outputTemplate: output))
            //        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.File(LogFilePath(nameof(LogEventLevel.Error), YamlBase), rollingInterval: RollingInterval.Day, outputTemplate: output))
            //        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Fatal).WriteTo.File(LogFilePath(nameof(LogEventLevel.Fatal), YamlBase), rollingInterval: RollingInterval.Day, outputTemplate: output))
            //        .CreateLogger();

            //    switch (eventLevel)
            //    {
            //        case LogEventLevel.Debug:
            //            Log.Debug(message);
            //            break;

            //        case LogEventLevel.Information:
            //            Log.Information(message);
            //            break;

            //        case LogEventLevel.Warning:
            //            Log.Warning(message);
            //            break;

            //        case LogEventLevel.Error:
            //            Log.Error(message);
            //            break;

            //        case LogEventLevel.Fatal:
            //            Log.Fatal(message);
            //            break;
            //    }

            //    static string LogFilePath(string LogEvent, MainBox box) => $"{AppContext.BaseDirectory}..\\{box.LogFileName}\\{LogEvent}\\{box.LogDocumentName}";
            //}
            //catch (Exception e)
            //{
            //    Log.Error($"{e.Message}\n{e.StackTrace}");
            //    throw;
            //}
            //finally
            //{
            //    Log.CloseAndFlush();
            //}
        }
    }
}
