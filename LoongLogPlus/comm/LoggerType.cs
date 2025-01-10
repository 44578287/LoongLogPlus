namespace LoongLogPlus.comm;

/// <summary>
/// 日志类型
/// </summary>
[Flags]
public enum LoggerType
{
    /// <summary>
    /// 调试版
    /// </summary>
    Debug = 0x00001, // 或者使用1， 即二进制的0001

    /// <summary>
    /// 控制台版
    /// </summary>
    Console = 0x00010, // 或者使用2， 即二进制的0010

    /// <summary>
    /// 文件版
    /// </summary>
    File = 0x00100, // 或者使用4， 即二进制的0100

    /// <summary>
    /// 内存版
    /// </summary>
    Memory = 0x01000, // 或者使用8， 即二进制的1000

    /// <summary>
    /// SQL版
    /// </summary>
    Sqlite = 0x10000 // 或者使用16， 即二进制的10000
}