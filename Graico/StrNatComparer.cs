using System;
 
/*
strnatcmp.cs
Copyright (C) 2013 by DOBON! <https://dobon.net>
 
Based on
strnatcmp.c -- Perform 'natural order' comparisons of strings in C.
Copyright (C) 2000, 2004 by Martin Pool <mbp sourcefrog net>
 
This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.
 
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
 
1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/
 
/// <summary>
/// 自然順アルゴリズムにより文字列比較を行います。
/// </summary>
public class StrNatComparer :
    System.Collections.IComparer,
    System.Collections.Generic.IComparer<string>
{
    private bool ignoreCase = false;
 
    /// <summary>
    /// StrNatComparerのコンストラクタ。
    /// </summary>
    /// <param name="ignoreCaseComparer">
    /// 大文字小文字を区別しない文字列比較を行います
    /// </param>
    public StrNatComparer(bool ignoreCaseComparer)
    {
        this.ignoreCase = ignoreCaseComparer;
    }
    public StrNatComparer()
        : this(false)
    {
    }
 
    public int Compare(string a, string b)
    {
        return StrNatCmp0(a, b, this.ignoreCase);
    }
 
    public int Compare(object a, object b)
    {
        return this.Compare(a.ToString(), b.ToString());
    }
 
    /// <summary>
    /// 指定した文字列内の指定したインデックスの文字を返します。
    /// </summary>
    /// <param name="s">対象とする文字列。</param>
    /// <param name="index">取得したい文字があるインデックス。</param>
    /// <returns>指定したインデックスに文字があれば、その文字。
    /// なければ、'\0'。</returns>
    private static char GetChar(string s, int index)
    {
        if ((index < 0) || (s.Length <= index))
        {
            return '\0';
        }
        return s[index];
    }
 
    /* These are defined as macros to make it easier to adapt this code to
       different characters types or comparison functions. */
    private static bool IsDigit(char c)
    {
        return ('0' <= c) && (c <= '9');
    }
 
    private static bool IsSpace(char c)
    {
        return char.IsWhiteSpace(c);
    }
 
    private static char ToUpper(char c)
    {
        return char.ToUpper(c,
            System.Globalization.CultureInfo.InvariantCulture);
    }
 
    private static int CompareRight(string a, string b, int ai, int bi)
    {
        int bias = 0;
 
        /* The longest run of digits wins.  That aside, the greatest
        value wins, but we can't know that it will until we've scanned
        both numbers to know that they have the same magnitude, so we
        remember it in BIAS. */
        for (; ; ai++, bi++)
        {
            char ca = GetChar(a, ai);
            char cb = GetChar(b, bi);
 
            if (!IsDigit(ca) && !IsDigit(cb))
            {
                return bias;
            }
            else if (!IsDigit(ca))
            {
                return -1;
            }
            else if (!IsDigit(cb))
            {
                return 1;
            }
            else if (ca < cb)
            {
                if (bias != 0)
                {
                    bias = -1;
                }
            }
            else if (ca > cb)
            {
                if (bias != 0)
                {
                    bias = 1;
                }
            }
            else if (ca == '\0' && cb == '\0')
            {
                return bias;
            }
        }
    }
 
    private static int CompareLeft(string a, string b, int ai, int bi)
    {
        /* Compare two left-aligned numbers: the first to have a
           different value wins. */
        for (; ; ai++, bi++)
        {
            char ca = GetChar(a, ai);
            char cb = GetChar(b, bi);
 
            if (!IsDigit(ca) && !IsDigit(cb))
            {
                return 0;
            }
            else if (!IsDigit(ca))
            {
                return -1;
            }
            else if (!IsDigit(cb))
            {
                return 1;
            }
            else if (ca < cb)
            {
                return -1;
            }
            else if (ca > cb)
            {
                return 1;
            }
        }
    }
 
    private static int StrNatCmp0(string a, string b, bool foldCase)
    {
        if (a == null)
        {
            if (b == null)
            {
                return 0;
            }
            return -1;
        }
        if (b == null)
        {
            return 1;
        }
 
        int ai = 0;
        int bi = 0;
 
        while (true)
        {
            char ca = GetChar(a, ai);
            char cb = GetChar(b, bi);
 
            /* skip over leading spaces or zeros */
            while (char.IsWhiteSpace(ca))
            {
                ai++;
                ca = GetChar(a, ai);
            }
 
            while (char.IsWhiteSpace(cb))
            {
                bi++;
                cb = GetChar(b, bi);
            }
 
            /* process run of digits */
            if (IsDigit(ca) && IsDigit(cb))
            {
                bool fractional = (ca == '0' || cb == '0');
                if (fractional)
                {
                    int result = CompareLeft(a, b, ai, bi);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                else
                {
                    int result = CompareRight(a, b, ai, bi);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }
 
            if (ca == '\0' && cb == '\0')
            {
                /* The strings compare the same.  Perhaps the caller
                       will want to call strcmp to break the tie. */
                return 0;
            }
 
            if (foldCase)
            {
                ca = char.ToUpper(ca);
                cb = char.ToUpper(cb);
            }
 
            if (ca < cb)
            {
                return -1;
            }
            else if (ca > cb)
            {
                return 1;
            }
 
            ai++;
            bi++;
        }
    }
 
    public int StrNatCmp(string a, string b)
    {
        return StrNatCmp0(a, b, false);
    }
 
    /* Compare, recognizing numeric string and ignoring case. */
    public int StrNatCaseCmp(string a, string b)
    {
        return StrNatCmp0(a, b, true);
    }
}
