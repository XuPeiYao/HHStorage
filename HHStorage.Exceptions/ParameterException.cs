using System;
using System.Collections.Generic;
using System.Text;

namespace HHStorage.Exceptions {
    /// <summary>
    /// 參數錯誤
    /// </summary>
    public class ParameterException : ExceptionBase {
        /// <summary>
        /// 建立格式錯誤例外實例
        /// </summary>
        /// <param name="message">詳細訊息</param>
        public ParameterException(string message = "遺漏參數或參數錯誤") : base("參數錯誤", message) { }
    }
}
