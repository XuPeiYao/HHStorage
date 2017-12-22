using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HHStorage.Models.EF {
    /// <summary>
    /// 存取範圍
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccessModifierTypes {
        /// <summary>
        /// 公開
        /// </summary>
        Public,
        /// <summary>
        /// 僅目前使用者
        /// </summary>
        Private,
        /// <summary>
        /// 系統內所有使用者
        /// </summary>
        Protected
    }
}
