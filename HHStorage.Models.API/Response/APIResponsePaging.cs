using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHStorage.Models.API.Response {
    /// <summary>
    /// 分頁資訊
    /// </summary>
    public class APIResponsePaging {
        /// <summary>
        /// 起始索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 獲取長度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 分頁項目總長
        /// </summary>
        public int TotalLength { get; set; }
    }
}
