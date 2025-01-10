using LoongLogPlus.comm;

namespace LoongLogPlus;

internal class ConsoleLogger : BaseLogger
{
    public ConsoleLogger(LoggerLevel level = LoggerLevel.Debug) : base(level)
    {
    }

    public override bool WriteLine(string fullMessage, MessageType type)
    {
        if ((int)type < (int)Level)
            return false;

        var boldColor = Console.BackgroundColor;
        var oldColor = Console.ForegroundColor;

        switch (type)
        {
            case MessageType.Debug: //调试
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;

            case MessageType.Info: //一般
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                break;

            case MessageType.Warn: //警告
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                break;

            case MessageType.Error: //错误
                Console.ForegroundColor = ConsoleColor.Red;
                break;

            case MessageType.Fatal: //致命
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                break;
        }

        Console.WriteLine(fullMessage);
        Console.BackgroundColor = boldColor;
        Console.ForegroundColor = oldColor;
        return true;
    }
}