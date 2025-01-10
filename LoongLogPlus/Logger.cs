using System.Runtime.CompilerServices;
using LoongLogPlus.comm;

namespace LoongLogPlus;

/// <summary>
/// 高贵的日志库
/// </summary>
public class Logger
{
    private static readonly List<BaseLogger> Loggers = [];

    /// <summary>
    /// 日志所属ID
    /// </summary>
    public static string SessionId { get; private set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 使能各个Logger
    /// </summary>
    /// <param name="type">需要开启的Logger类型，可以使用“|”位域操作</param>
    /// <param name="level">开启的Logger的级别</param> 
    /// <param name="logPath">指定log存放位置</param>
    /// <param name="dbPath">指定dblog存放位置</param> 
    /// <param name="maxCapacity">最大日志数量</param> 
    /// <example>
    ///     // 开启调试输出和控制台的Logger，消息级别为Error
    ///     LoggerManager.Enable(LoggerType.Debug | LoggerType.Console,  LoggerLevel.Error);
    /// </example>
    /// <code>
    ///     LoggerManager.Enable(LoggerType.Debug | LoggerType.Console,  LoggerLevel.Error);
    /// </code>
    public static void Enable(LoggerType type, LoggerLevel level = LoggerLevel.Debug, string? logPath = null,
        string dbPath = "logs.db", int maxCapacity = 100)
    {
        Change(type, level, logPath, dbPath, maxCapacity);
        Info($"日志开始({SessionId}) 项目发布地址 https://445720.xyz and https://github.com/44578287 by ck小捷");
    }

    /// <summary>
    /// 销毁所有的Logger
    /// </summary>
    public static void Disable()
    {
        Info($"日志结束({SessionId})");
        Loggers.Clear();
    }

    /// <summary>
    /// 更改Logger的类型
    /// </summary>
    /// <param name="type">需要开启的Logger类型，可以使用“|”位域操作</param>
    /// <param name="level">开启的Logger的级别</param> 
    /// <param name="logPath">指定log存放位置</param>
    /// <param name="dbPath">指定dblog存放位置</param> 
    /// <param name="maxCapacity">最大日志数量</param> 
    public static void Change(LoggerType type, LoggerLevel level = LoggerLevel.Debug, string? logPath = null,
        string dbPath = "logs.db", int maxCapacity = 100)
    {
        Loggers.Clear();

        if (type.HasFlag(LoggerType.Console))
            Loggers.Add(new ConsoleLogger(level));

        if (type.HasFlag(LoggerType.Debug))
            Loggers.Add(new DebugLogger(level));

        if (type.HasFlag(LoggerType.File))
            Loggers.Add(new FileLogger(level, logPath));

        if (type.HasFlag(LoggerType.Memory))
            Loggers.Add(new MemoryLogger(level, maxCapacity));

        if (type.HasFlag(LoggerType.Sqlite))
            Loggers.Add(new SqliteLogger(level, dbPath));
    }

    /// <summary>
    /// 删除指定类型的Logger
    /// </summary>
    /// <param name="type">指定类型的Logger</param>
    public static void RemovingType(LoggerType type)
    {
        if (type.HasFlag(LoggerType.Console))
            Loggers.RemoveAll(x => x is ConsoleLogger);

        if (type.HasFlag(LoggerType.Debug))
            Loggers.RemoveAll(x => x is DebugLogger);

        if (type.HasFlag(LoggerType.File))
            Loggers.RemoveAll(x => x is FileLogger);

        if (type.HasFlag(LoggerType.Memory))
            Loggers.RemoveAll(x => x is MemoryLogger);

        if (type.HasFlag(LoggerType.Sqlite))
            Loggers.RemoveAll(x => x is SqliteLogger);
    }

    private static readonly object Lock = new object();

    /// <summary>
    /// 日志变化事件
    /// </summary>
    public static Action<Log>? OnLogChanged
    {
        get => MemoryLogger.OnLogChanged;
        set => MemoryLogger.OnLogChanged = value;
    }

    /// <summary>
    /// 获取所有的日志
    /// </summary>
    /// <returns></returns>
    public static List<Log> GetLogs() => MemoryLogger.GetAllLogs();

    /// <summary>
    /// 获取指定类型的日志
    /// </summary>
    /// <param name="type">日志等级</param>
    /// <returns>匹配的日志列表</returns>
    public static List<Log> GetLogsByType(MessageType type) => MemoryLogger.GetLogsByType(type);

    /// <summary>
    /// 按时间范围筛选日志
    /// </summary>
    /// <param name="startTime">起始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>匹配的日志列表</returns>
    public static List<Log> GetLogsByTimeRange(DateTime startTime, DateTime endTime) =>
        MemoryLogger.GetLogsByTimeRange(startTime, endTime);

    /// <summary>
    /// 按调用方法名筛选日志
    /// </summary>
    /// <param name="callerName">调用方法名</param>
    /// <returns>匹配的日志列表</returns>
    public static List<Log> GetLogsByCallerName(string callerName) => MemoryLogger.GetLogsByCallerName(callerName);

    /// <summary>
    /// 清空所有的日志
    /// </summary>
    public static void ClearLogs() => MemoryLogger.ClearLogs();

    /// <summary>
    /// 打印一条新的日志消息
    /// </summary>
    ///     <param name="type">消息类型</param>
    ///     <param name="message">消息的具体内容</param>
    ///     <param name="isDetailMode">详细模式？</param>
    ///     <param name="callerName">调用的方法的名字</param>
    ///     <param name="fileName">调用方法所在的文件名</param>
    ///     <param name="line">调用代码所在行</param>
    /// <returns>[true]->打印成功</returns>
    private static bool WriteLine
    (
        MessageType type,
        string? message,
        bool isDetailMode,
        string? callerName,
        string? fileName,
        int line
    )
    {
        bool isWrited = false;

        // TODO: 2020-04-26 增加了线程锁
        lock (Lock)
        {
            var msg = BaseLogger.FormatMessage(type, message, isDetailMode, callerName, fileName, line);

            if (Loggers.Count != 0)
            {
                isWrited = true;
                Loggers.ForEach(logger =>
                {
                    isWrited &= logger switch
                    {
                        MemoryLogger memoryLogger => memoryLogger.WriteLine(type, message, callerName, fileName, line),
                        SqliteLogger sqliteLogger => sqliteLogger.WriteLine(type, message, callerName, fileName, line),
                        _ => logger.WriteLine(msg, type)
                    };
                });
            }
        }

        return isWrited;
    }

    /// <summary>
    /// 打印一条新的调试信息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="isDetailMode">详细模式？</param>
    /// <param name="callerName">调用的方法的名字</param>
    /// <param name="fileName">调用方法所在的文件名</param>
    /// <param name="line">调用代码所在行</param>
    /// <returns>[true]->打印成功</returns>
    public static bool Debug
    (
        string message,
        bool isDetailMode = false,
        [CallerMemberName] string? callerName = null,
        [CallerFilePath] string? fileName = null,
        [CallerLineNumber] int line = 0
    ) => WriteLine(MessageType.Debug, message, isDetailMode, callerName, fileName, line);

    /// <summary>
    /// 打印一条新的一般信息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="isDetailMode">详细模式？</param>
    /// <param name="callerName">调用的方法的名字</param>
    /// <param name="fileName">调用方法所在的文件名</param>
    /// <param name="line">调用代码所在行</param>
    /// <returns>[true]->打印成功</returns>
    public static bool Info
    (
        string message,
        bool isDetailMode = false,
        [CallerMemberName] string? callerName = null,
        [CallerFilePath] string? fileName = null,
        [CallerLineNumber] int line = 0
    ) => WriteLine(MessageType.Info, message, isDetailMode, callerName, fileName, line);

    /// <summary>
    /// 打印一条新的警告信息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="isDetailMode">详细模式？</param>
    /// <param name="callerName">调用的方法的名字</param>
    /// <param name="fileName">调用方法所在的文件名</param>
    /// <param name="line">调用代码所在行</param>
    /// <returns>[true]->打印成功</returns>
    public static bool Warn
    (
        string message,
        bool isDetailMode = false,
        [CallerMemberName] string? callerName = null,
        [CallerFilePath] string? fileName = null,
        [CallerLineNumber] int line = 0
    ) => WriteLine(MessageType.Warn, message, isDetailMode, callerName, fileName, line);

    /// <summary>
    /// 打印一条新的错误信息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="isDetailMode">详细模式？</param>
    /// <param name="callerName">调用的方法的名字</param>
    /// <param name="fileName">调用方法所在的文件名</param>
    /// <param name="line">调用代码所在行</param>
    /// <returns>[true]->打印成功</returns>
    public static bool Error
    (
        string message,
        bool isDetailMode = true,
        [CallerMemberName] string? callerName = null,
        [CallerFilePath] string? fileName = null,
        [CallerLineNumber] int line = 0
    ) => WriteLine(MessageType.Error, message, isDetailMode, callerName, fileName, line);

    /// <summary>
    /// 打印一条新的崩溃信息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="isDetailMode">详细模式？</param>
    /// <param name="callerName">调用的方法的名字</param>
    /// <param name="fileName">调用方法所在的文件名</param>
    /// <param name="line">调用代码所在行</param>
    /// <returns>[true]->打印成功</returns>
    public static bool Fatal
    (
        string message,
        bool isDetailMode = true,
        [CallerMemberName] string? callerName = null,
        [CallerFilePath] string? fileName = null,
        [CallerLineNumber] int line = 0
    ) => WriteLine(MessageType.Fatal, message, isDetailMode, callerName, fileName, line);
}