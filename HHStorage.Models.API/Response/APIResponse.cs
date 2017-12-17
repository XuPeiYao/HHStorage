using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EzCoreKit.Extensions;
using Newtonsoft.Json;

namespace HHStorage.Models.API.Response {
    /// <summary>
    /// API回應規格
    /// </summary>
    /// <typeparam name="TResult">結果類型</typeparam>
    public class APIResponse {
        /// <summary>
        /// 是否成功請求
        /// </summary>
        [Required]
        public bool Success { get; set; } = true;

        /// <summary>
        /// API運作結果中介資料
        /// </summary>
        [Required]
        public APIResponseTypeMeta Meta => APIResponseTypeMeta.GetTypeMeta(this.Result);

        private object result;

        /// <summary>
        /// API運作結果
        /// </summary>
        [Required]
        public object Result {
            get {
                return result;
            }
            set {
                if (value != null && (value is Exception || value is APIError)) {
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
        [JsonIgnore]
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// Javascript中的Date時間值
        /// </summary>
        [JsonProperty("time")]
        private long JSTime {
            get {
                return Time.ToUnixTimestamp() * 1000;
            }
            set {
                Time = DateTimeConvert.FromUnixTimestamp(value / 1000);
            }
        }
    }
}