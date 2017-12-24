using System;
using System.Collections.Generic;
using System.Text;

namespace HHStorage.Models.API.Request {
    /// <summary>
    /// 檔案存取權杖類型
    /// </summary>
    public enum FileTokenTypes {
        /// <summary>
        /// 下載
        /// </summary>
        Download,
        /// <summary>
        /// 上傳
        /// </summary>
        Upload
    }
}
