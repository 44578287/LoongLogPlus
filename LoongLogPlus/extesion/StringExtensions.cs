using System.Text;

namespace LoongLogPlus.extesion;

/// <summary>
/// 字符串扩展
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 把字符型两侧加上****直到指定的总长度
    /// </summary>
    ///     <param name="self">字符串本身</param>
    ///     <param name="width">指定的总长度</param>
    /// <returns>
    ///     格式化后的字符串
    /// </returns>
    public static string ToHeader(this string self, int width)
    {
        if (self.Length >= width)
        {
            return self;
        }
        else
        {
            var msg = new StringBuilder(self);

            // msg是奇数？

            var isOdd = width % 2 != 0;
            if (isOdd) width += 1;

            int startCount;
            isOdd = self.Length % 2 != 0;
            if (isOdd)
            {
                startCount = (width - self.Length - 1) / 2;
                msg.Append(' ');
            }
            else
            {
                startCount = (width - self.Length) / 2;
            }

            msg.Append('*', startCount);
            msg.Append(Environment.NewLine);
            return msg.ToString().PadLeft(width, '*');
        }
    }


    /// <summary>
    /// 把字符型两侧加上****直到指定的总长度
    /// </summary>
    ///     <param name="self">字符串本身</param>
    ///     <param name="width">指定的总长度</param>
    /// <returns>
    ///     格式化后的字符串
    /// </returns>
    public static string ToContent(this string self, int width)
    {
        if (self.Length >= width - 2)
        {
            return self;
        }
        else
        {
            var msg = new StringBuilder(self);

            // 判断指定的长度为奇数？
            var isOdd = width % 2 != 0;

            // 把长度悄悄变为偶数
            if (isOdd) width += 1;

            // 判断原字符串的长度为奇数？
            isOdd = self.Length % 2 != 0;
            int spaceCount;
            if (isOdd)
            {
                spaceCount = (width - self.Length - 1) / 2;
                msg.Append(' ');
            }
            else
            {
                spaceCount = (width - self.Length) / 2;
            }

            msg.Append(' ', spaceCount - 1);
            msg.Append("*" + Environment.NewLine);
            return "*" + msg.ToString().PadLeft(width - 1, ' ');
        }
    }

    /// <summary>
    /// 把字符型两侧加上 直到指定的总长度
    /// </summary>
    ///     <param name="self">字符串本身</param>
    ///     <param name="width">指定的总长度</param>
    /// <returns>
    ///     格式化后的字符串
    /// </returns>
    public static string ToContent2(this string self, int width)
    {
        if (self.Length >= width - 2)
        {
            return self;
        }
        else
        {
            var msg = new StringBuilder(self);

            // 判断指定的长度为奇数？
            var isOdd = width % 2 != 0;

            // 把长度悄悄变为偶数
            if (isOdd) width += 1;

            // 判断原字符串的长度为奇数？
            isOdd = self.Length % 2 != 0;
            int spaceCount;
            if (isOdd)
            {
                spaceCount = (width - self.Length - 1) / 2;
                msg.Append(' ');
            }
            else
            {
                spaceCount = (width - self.Length) / 2;
            }

            msg.Append(' ', spaceCount - 1);
            msg.Append("" + Environment.NewLine);
            return "" + msg.ToString().PadLeft(width - 1, ' ');
        }
    }
}