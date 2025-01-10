using LoongLogPlus.comm;

namespace LoongLogPlus;

internal class FileLogger : BaseLogger
{
    // Logger文件所在的路径
    public string FilePath { get; private set; }

    public FileLogger(LoggerLevel level = LoggerLevel.Debug, string? filePath = null) : base(level)
    {
        if (filePath == null)
        {
            // TODO: 10-A 获取程序运行的根目录
            var root = Environment.CurrentDirectory;

            // TODO: 10-B 创建Log文件夹
            if (!Directory.Exists(root + @"/log/"))
            {
                Directory.CreateDirectory(root + @"/log/");
            }

            this.FilePath = root + @"/log/" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + $" {Logger.SessionId}" + ".log";
        }
        else
        {
            this.FilePath = filePath;
        }

        // TODO: 10-C 创建log文件
        // using (StreamWriter writer = new StreamWriter(this.FilePath))
        // {
        //     writer.WriteLineAsync(BaseLogger.FormatMessage(MessageType.Info, "Logger File is Created...", true, nameof(FileLogger), "Created by Constructor", 46));
        // }
    }

    private static readonly object Lock = new object();

    public override bool WriteLine(string fullMessage, MessageType type)
    {
        // TODO: [Update] 2020-04-26 加了线程锁
        lock (Lock)
        {
            if ((int)type < (int)Level)
                return false;
            using var writer = new StreamWriter(this.FilePath, true);
            writer.WriteLineAsync(fullMessage);
        }

        return true;
    }
}