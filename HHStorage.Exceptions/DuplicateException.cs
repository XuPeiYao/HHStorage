using System;
using System.Collections.Generic;
using System.Text;

namespace HHStorage.Exceptions {
    /// <summary>
    /// 重複項目例外
    /// </summary>
    public class DuplicateException : ExceptionBase {
        /// <summary>
        /// 建立重複項目例外實例
        /// </summary>
        /// <param name="message">詳細訊息</param>
        public DuplicateException(string message = "新增實例之唯一識別號重複") : base("重複項目", message) { }

    }
}
