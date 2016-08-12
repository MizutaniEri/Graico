using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FindAssocExe
{
    public static class FindAssocExe
    {
        /// <summary>
        /// 指定された拡張子に関連付けられた実行ファイルのパスを取得する。
        /// </summary>
        /// <param name="extName">".txt"などの拡張子。</param>
        /// <returns>見つかった時は、実行ファイルのパス。
        /// 見つからなかった時は、空の文字列。</returns>
        /// <example>
        /// 拡張子".txt"に関連付けられた実行ファイルのパスを取得する例
        /// <code>
        /// string exePath = FindAssociatedExecutable(".txt");
        /// </code>
        /// </example>
        public static string FindAssociatedExecutable(this string extName)
        {
            //pszOutのサイズを取得する
            uint pcchOut = 0;
            //ASSOCF_INIT_IGNOREUNKNOWNで関連付けられていないものを無視
            //ASSOCF_VERIFYを付けると検証を行うが、パフォーマンスは落ちる
            AssocQueryString(AssocF.Init_IgnoreUnknown, AssocStr.Executable,
                extName, null, null, ref pcchOut);
            if (pcchOut == 0)
            {
                return string.Empty;
            }
            //結果を受け取るためのStringBuilderオブジェクトを作成する
            StringBuilder pszOut = new StringBuilder((int)pcchOut);
            //関連付けられた実行ファイルのパスを取得する
            AssocQueryString(AssocF.Init_IgnoreUnknown, AssocStr.Executable,
                extName, null, pszOut, ref pcchOut);
            //結果を返す
            return pszOut.ToString();
        }

        [DllImport("Shlwapi.dll",
            SetLastError = true,
            CharSet = CharSet.Auto)]
        private static extern uint AssocQueryString(AssocF flags,
            AssocStr str,
            string pszAssoc,
            string pszExtra,
            [Out] StringBuilder pszOut,
            [In][Out] ref uint pcchOut);

        [Flags]
        private enum AssocF
        {
            None = 0,
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200,
            Init_IgnoreUnknown = 0x400,
            Init_FixedProgId = 0x800,
            IsProtocol = 0x1000,
            InitForFile = 0x2000,
        }

        private enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic,
            InfoTip,
            QuickTip,
            TileInfo,
            ContentType,
            DefaultIcon,
            ShellExtension,
            DropTarget,
            DelegateExecute,
            SupportedUriProtocols,
            Max,
        }
    }
}