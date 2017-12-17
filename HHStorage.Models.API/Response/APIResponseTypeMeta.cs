using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HHStorage.Models.API.Response {
    /// <summary>
    /// API運作結果中介資料
    /// </summary>
    public class APIResponseTypeMeta {
        /// <summary>
        /// 類型種類
        /// </summary>
        [Required]
        public APIResponseTypes Type { get; set; } = APIResponseTypes.Object;

        /// <summary>
        /// 類型名稱
        /// </summary>
        [Required]
        public string TypeName { get; set; } = nameof(Object);

        /// <summary>
        /// 元素中介資料
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public APIResponseTypeMeta ElementMeta { get; set; }

        /// <summary>
        /// 取得輸入物件的型別中介資料
        /// </summary>
        /// <param name="obj">輸入物件</param>
        /// <returns>輸入物件產生的中介資料</returns>
        public static APIResponseTypeMeta GetTypeMeta(object obj) {
            var result = new APIResponseTypeMeta();

            if (obj == null) {
                return result;
            }

            result.Type = obj is Array ? APIResponseTypes.Array : APIResponseTypes.Object;

            if (result.Type == APIResponseTypes.Array) {
                result.TypeName = "Array";
                Array arrayObj = (Array)obj;

                if (arrayObj.Length > 0) {
                    object firstNotNullElement = null;
                    foreach (var element in arrayObj) {
                        if (element == null) continue;
                        firstNotNullElement = element;
                    }
                    result.ElementMeta = GetTypeMeta(firstNotNullElement);
                }
            } else {
                result.TypeName = obj.GetType().Name;
            }

            return result;
        }
    }
}
