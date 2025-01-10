using LoongLogPlus;
using LoongLogPlus.comm;

Logger.Enable(LoggerType.Console | LoggerType.Memory | LoggerType.File | LoggerType.Debug | LoggerType.Sqlite, LoggerLevel.Error);

Logger.Debug("Hello World!");
Logger.Info("Hello World!");
Logger.Warn("Hello World!");
Logger.Error("Hello World!");
Logger.Fatal("Hello World!");

var data = Logger.GetLogs();
foreach (var log in data)
{
    Console.WriteLine(log.Message);
}

Logger.Disable();