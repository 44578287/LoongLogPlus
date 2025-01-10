using System.Diagnostics;
using LoongLogPlus.comm;

namespace LoongLogPlus;

internal class DebugLogger : BaseLogger
{
    public DebugLogger(LoggerLevel level = LoggerLevel.Debug) : base(level)
    {
    }

    public override bool WriteLine(string fullMessage, MessageType type)
    {
        Debug.WriteLine(fullMessage);
        return true;
    }
}