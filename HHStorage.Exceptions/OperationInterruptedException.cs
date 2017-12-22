using System;
using System.Collections.Generic;
using System.Text;

namespace HHStorage.Exceptions {
    /// <summary>
    /// 操作中斷例外
    /// </summary>
    public class OperationInterruptedException : ExceptionBase {
        /// <summary>
        /// 建立操作中斷例外實例
        /// </summary>
        /// <param name="message">詳細訊息</param>
        public OperationInterruptedException(string message = "執行操作過程中意外中斷") : base("操作中斷", message) {
        }
    }
}
