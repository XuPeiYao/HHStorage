using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HHStorage.Exceptions {
    [JsonObject(MemberSerialization.OptIn)]
    public class ExceptionBase : Exception {
        /// <summary>
        /// 主題
        /// </summary>
        [JsonProperty]
        public string Subject { get; set; }

        /// <summary>
        /// 詳細訊息
        /// </summary>
        [JsonProperty]
        public new string Message { get; set; }


        public ExceptionBase(string subject, string message) {
            this.Subject = subject;
            this.Message = message;
        }
    }
}
