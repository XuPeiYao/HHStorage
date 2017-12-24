using System;
using System.Collections.Generic;
using System.Text;

namespace HHStorage.Exceptions {
    /// <summary>
    /// 認證失敗
    /// </summary>
    public class AuthorizeException : ExceptionBase {
        /// <summary>
        /// 建立認證失敗實例
        /// </summary>
        /// <param name="message">詳細訊息</param>
        public AuthorizeException(string message = "您的身分或輸入的帳號密碼錯誤") : base("認證失敗", message) { }
    }
}
