using System;
using System.Collections.Generic;
using System.Text;
using HHStorage.Exceptions;

namespace HHStorage.Exceptions {
    /// <summary>
    /// 項目不該為空值例外
    /// </summary>
    public class NotNullException : ExceptionBase {
        /// <summary>
        /// 建立項目不該為空值例外實例
        /// </summary>
        /// <param name="message">詳細訊息</param>
        public NotNullException(string message = "您輸入的項目或參數不該為null") : base("項目不該為空值", message) {
        }
    }
}
