using System;
using System.Collections.Generic;
using System.Text;

namespace HHStorage.Exceptions {
    /// <summary>
    /// 操作例外
    /// </summary>
    public class OperationException : ExceptionBase {
        /// <summary>
        /// 建立項目操作例外實例
        /// </summary>
        /// <param name="message">詳細訊息</param>
        public OperationException(string message = "您執行了不正確的操作") : base("操作例外", message) {
        }
    }
}
