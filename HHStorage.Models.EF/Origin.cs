using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HHStorage.Models.EF {
    /// <summary>
    /// 存取域
    /// </summary>
    public partial class Origin {
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
        /// 存取域網址
        /// </summary>
        public string OriginURI { get; set; }

        /// <summary>
        /// 儲存庫
        /// </summary>
        [JsonIgnore]
        public Repository Repository { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        [JsonIgnore]
        public User User { get; set; }
    }
}
