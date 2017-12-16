using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HHStorage.Models.API.Response {
    /// <summary>
    /// API回應規格
    /// </summary>
    /// <typeparam name="TResult">結果類型</typeparam>
    public class APIResponse<TResult> {
        /// <summary>
        /// 是否成功請求
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// API運作結果中介資料
        /// </summary>
        public APIResponseTypeMeta Meta => APIResponseTypeMeta.GetTypeMeta(this.Result);

        private TResult result;

        /// <summary>
        /// API運作結果
        /// </summary>
        public TResult Result {
            get {
                return result;
            }
            set {
                if (value != null && value is Exception) {
                    Success = false;
                }
                result = value;
            }
        }

        /// <summary>
        /// 分頁資訊
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public APIResponsePaging Paging { get; set; }

        /// <summary>
        /// 回應發生時間
        /// </summary>
        public DateTime Time => DateTime.Now;
    }
}
