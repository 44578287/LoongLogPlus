namespace LoongLogPlus.comm;

/// <summary>
/// 日志内容实体
/// </summary>
public class Log
{
    /// <summary>
    /// 消息时间
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageType Type { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 调用方法的名字
    /// </summary>
    public string? CallerName { get; set; }

    /// <summary>
    /// 调用的文件名
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 调用代码所在行
    /// </summary>
    public int Line { get; set; }
}