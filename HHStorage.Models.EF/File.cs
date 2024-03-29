﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace HHStorage.Models.EF {
    /// <summary>
    /// 檔案
    /// </summary>
    public partial class File {
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
        public Guid RepositoryId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 檔案大小
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// 存取限制詞
        /// </summary>
        [JsonIgnore]
        [Column("AccessModifier")]
        public string AccessModifierString { get; set; } = "Private";

        [NotMapped]
        public AccessModifierTypes AccessModifier {
            get {
                return Enum.Parse<AccessModifierTypes>(AccessModifierString);
            }
            set {
                AccessModifierString = value.ToString();
            }
        }

        /// <summary>
        /// 存取限制詞
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
