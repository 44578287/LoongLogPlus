using System.Data.SQLite;
using Dapper;
using LoongLogPlus.comm;

namespace LoongLogPlus;

internal class SqliteLogger : BaseLogger
{
    private readonly string _connectionString;
    private readonly int _sessionDbId;

    public SqliteLogger(LoggerLevel level, string databasePath = "logs.db")
        : base(level)
    {
        _connectionString = $"Data Source={databasePath}";

        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();

            // 创建 Sessions 表
            const string createSessionsTable = @"
                CREATE TABLE IF NOT EXISTS Sessions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SessionId TEXT NOT NULL UNIQUE,
                    StartTime INTEGER NOT NULL
                );";

            // 创建 Logs 表
            const string createLogsTable = @"
                CREATE TABLE IF NOT EXISTS Logs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Time INTEGER NOT NULL,
                    Type TEXT NOT NULL,
                    Message TEXT,
                    CallerName TEXT,
                    FileName TEXT,
                    Line INTEGER NOT NULL,
                    SessionId INTEGER NOT NULL,
                    FOREIGN KEY (SessionId) REFERENCES Sessions(Id)
                );";

            connection.Execute(createSessionsTable);
            connection.Execute(createLogsTable);

            // 插入新启动会话
            var insertSessionQuery = @"
                INSERT INTO Sessions (SessionId, StartTime)
                VALUES (@SessionId, @StartTime);
                SELECT last_insert_rowid();";

            _sessionDbId = connection.ExecuteScalar<int>(insertSessionQuery, new
            {
                Logger.SessionId,
                StartTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()
            });
        }
    }

    public override bool WriteLine(string fullMessage, MessageType type)
    {
        // 不直接调用此方法
        return false;
    }

    /// <summary>
    /// 打印一条新的消息
    /// </summary>
    /// <param name="type">消息类型</param>
    /// <param name="message">消息的具体内容</param>
    /// <param name="callerName">调用方法的名字</param>
    /// <param name="fileName">调用的文件名</param>
    /// <param name="line">调用代码所在行</param>
    /// <returns></returns>
    public bool WriteLine(
        MessageType type,
        string? message,
        string? callerName,
        string? fileName,
        int line)
    {
        try
        {
            if ((int)type < (int)Level)
                return false;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var insertQuery = @"
                    INSERT INTO Logs (Time, Type, Message, CallerName, FileName, Line, SessionId)
                    VALUES (@Time, @Type, @Message, @CallerName, @FileName, @Line, @SessionId);";

                var logEntry = new Log
                {
                    Time = DateTime.Now,
                    Type = type,
                    Message = message,
                    CallerName = callerName,
                    FileName = fileName,
                    Line = line
                };

                connection.Execute(insertQuery, new
                {
                    Time = new DateTimeOffset(logEntry.Time).ToUnixTimeMilliseconds(),
                    Type = logEntry.Type.ToString(),
                    logEntry.Message,
                    logEntry.CallerName,
                    logEntry.FileName,
                    logEntry.Line,
                    SessionId = _sessionDbId // 引用当前会话的主键
                });
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"写入SQL日志失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 查询某次启动的所有日志
    /// </summary>
    /// <param name="sessionId">启动的 SessionId</param>
    /// <returns>日志列表</returns>
    public IEnumerable<Log> GetLogsBySession(string sessionId)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            var query = @"
                SELECT Logs.* 
                FROM Logs
                INNER JOIN Sessions ON Logs.SessionId = Sessions.Id
                WHERE Sessions.SessionId = @SessionId;";

            return connection.Query<Log>(query, new { SessionId = sessionId });
        }
    }

    /// <summary>
    /// 查询所有启动会话
    /// </summary>
    /// <returns>启动会话列表</returns>
    public IEnumerable<dynamic> GetAllSessions()
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT * FROM Sessions;";
            return connection.Query(query);
        }
    }

    /// <summary>
    /// 按条件查询日志
    /// </summary>
    /// <param name="startTime">开始时间（Unix 毫秒时间戳）</param>
    /// <param name="endTime">结束时间（Unix 毫秒时间戳）</param>
    /// <param name="type">日志类型（可选）</param>
    /// <returns>匹配条件的日志列表</returns>
    public IEnumerable<Log> QueryLogs(long? startTime = null, long? endTime = null, string? type = null)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();

            var queryBuilder = new List<string>();
            if (startTime.HasValue)
                queryBuilder.Add("Time >= @StartTime");
            if (endTime.HasValue)
                queryBuilder.Add("Time <= @EndTime");
            if (!string.IsNullOrEmpty(type))
                queryBuilder.Add("Type = @Type");

            var whereClause = queryBuilder.Count > 0 ? "WHERE " + string.Join(" AND ", queryBuilder) : string.Empty;

            var query = $"SELECT * FROM Logs {whereClause};";

            return connection.Query<Log>(query, new
            {
                StartTime = startTime,
                EndTime = endTime,
                Type = type
            });
        }
    }
}