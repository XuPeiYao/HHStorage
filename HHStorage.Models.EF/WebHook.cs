using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HHStorage.Models.EF {
    /// <summary>
    /// 網頁掛勾
    /// </summary>
    public partial class WebHook {
        /// <summary>
        /// 唯一識別號
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 儲存庫唯一識別號
        /// </summary>
        public Guid? RepositoryId { get; set; }

        /// <summary>
        /// 網頁掛勾網址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 儲存庫
        /// </summary>
        [JsonIgnore]
        public Repository Repository { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public User User { get; set; }
    }
}
