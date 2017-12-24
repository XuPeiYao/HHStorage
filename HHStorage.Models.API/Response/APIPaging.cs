using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HHStorage.Models.API.Response {
    /// <summary>
    /// 分頁資訊
    /// </summary>
    public class APIPaging<TElement> {

        /// <summary>
        /// 起始索引
        /// </summary>
        [Required]
        public int Index { get; set; }

        /// <summary>
        /// 獲取長度
        /// </summary>
        [Required]
        public int Length { get; set; }

        /// <summary>
        /// 分頁項目總長
        /// </summary>
        [Required]
        public int TotalLength { get; set; }

        /// <summary>
        /// 分頁內容
        /// </summary>
        [Required]
        public IEnumerable<TElement> Result { get; set; }
    }
}
