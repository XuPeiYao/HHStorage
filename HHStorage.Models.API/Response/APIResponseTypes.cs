using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HHStorage.Models.API.Response {
    /// <summary>
    /// API運算結果類型
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum APIResponseTypes {
        Object,
        Array
    }
}
