using System;
using System.Collections.Generic;
using System.Text;

namespace HHStorage.Exceptions {
    /// <summary>
    /// 找不到目標例外
    /// </summary>
    public class NotFoundException : ExceptionBase {
        /// <summary>
        /// 建立找不到目標例外實例
        /// </summary>
        /// <param name="message">詳細訊息</param>
        public NotFoundException(string message = "找不到指定的項目") : base("找不到目標", message) {
        }
    }
}
