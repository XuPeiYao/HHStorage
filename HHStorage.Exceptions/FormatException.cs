using System;
using System.Collections.Generic;
using System.Text;

namespace HHStorage.Exceptions {
    /// <summary>
    /// 格式錯誤例外
    /// </summary>
    public class FormatException : ExceptionBase {
        /// <summary>
        /// 建立格式錯誤例外實例
        /// </summary>
        /// <param name="message">詳細訊息</param>
        public FormatException(string message = "您輸入的資料格式錯誤") : base("格式錯誤", message) { }
    }
}
