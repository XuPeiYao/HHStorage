using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HHStorage.Models.EF {
    /// <summary>
    /// 使用者
    /// </summary>
    public partial class User {
        public User() {
            File = new HashSet<File>();
            Origin = new HashSet<Origin>();
            Repository = new HashSet<Repository>();
            WebHook = new HashSet<WebHook>();
        }

        /// <summary>
        /// 帳號
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// 檔案
        /// </summary>
        public ICollection<File> File { get; set; }

        /// <summary>
        /// 存取域
        /// </summary>
        public ICollection<Origin> Origin { get; set; }

        /// <summary>
        /// 儲存庫
        /// </summary>
        public ICollection<Repository> Repository { get; set; }

        /// <summary>
        /// 網頁掛勾
        /// </summary>
        public ICollection<WebHook> WebHook { get; set; }
    }
}
