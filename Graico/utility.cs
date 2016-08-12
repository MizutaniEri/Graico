using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graico
{
    public static class utility
    {
        public static bool IsNumeric(this string str)
        {
            int dmy;
            return Int32.TryParse(str, out dmy);
        }
    }

    /// <summary>
    /// for文、foreach文をなくすための、標準で存在しない拡張メソッド
    /// </summary>
    public static class EnumerableExtentions
    {
        /// <summary>
        /// index付、ForEach拡張メソッド
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int index = 0;
            foreach (var x in source)
                action(x, index++);
        }
        /// <summary>
        /// indexなし、ForEach拡張メソッド
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var x in source)
                action(x);
        }
    }
}
