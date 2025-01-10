namespace LoongLogPlus.comm;

/// <summary>
/// 日志消息类型定义
/// </summary>
public enum MessageType
{
    /// <summary>
    /// 调试信息
    /// </summary>
    Debug = 0,

    /// <summary>
    /// 一般信息
    /// </summary>
    Info = 1,

    /// <summary>
    /// 警告
    /// </summary>
    Warn = 2,

    /// <summary>
    /// 错误级
    /// </summary>
    Error = 3,

    /// <summary>
    /// 崩溃级
    /// </summary>
    Fatal = 4
}