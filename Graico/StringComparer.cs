using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Graico
{
    public class StringComparer : IComparer<string>
    {
        public static bool NumCheck = true;

        private static string _numRegex = @"^(.*?)([0-9]+).*?$";
        private static Regex regex = new Regex(_numRegex);

        //xがyより小さいときはマイナスの数、大きいときはプラスの数、
        //同じときは0を返す
        public int Compare(string a, string b)
        {
            string aorg = a;
            string borg = b;

            // 何もしなくても等しかったら０
            if (a == b)
            {
                return 0;
            }

            // 数字部分切り出し保存用
            long? ai = null;
            long? bi = null;

            // 数字チェックするなら
            if (NumCheck)
            {
                // 正規表現で切り出す
                Match matchCol = regex.Match(a);

                // マッチしたら
                if (matchCol.Success)
                {
                    // 数字の前までの文字列と
                    a = matchCol.Groups[1].Value;
                    // 数字に分ける
                    ai = Convert.ToInt64(matchCol.Groups[2].Value);
                }

                // 正規表現
                matchCol = regex.Match(b);
                // マッチ
                if (matchCol.Success)
                {
                    // 文字列
                    b = matchCol.Groups[1].Value;
                    // 数字
                    bi = Convert.ToInt64(matchCol.Groups[2].Value);
                }
            }

            // 文字列の比較
            int t = string.Compare(a, b);

            // 等しければ
            if (NumCheck && t == 0)
            {
                // 
                if (ai == null && bi != null)
                {
                    t = -1;
                }
                else if (ai != null && bi == null)
                {
                    t = 1;
                }
                else if (ai == null && bi == null)
                {
                    t = string.Compare(aorg, borg);
                }
                else
                {
                    t = (int)(ai - bi);
                    if (t == 0)
                    {
                        t = string.Compare(aorg, borg);
                    }
                }
            }

            return t;
        }
    }
}
