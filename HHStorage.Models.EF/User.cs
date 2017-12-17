using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HHStorage.Models.EF {
    public partial class User {
        public User() {
            File = new HashSet<File>();
            Origin = new HashSet<Origin>();
            Repository = new HashSet<Repository>();
            WebHook = new HashSet<WebHook>();
        }

        public string Id { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public ICollection<File> File { get; set; }
        public ICollection<Origin> Origin { get; set; }
        public ICollection<Repository> Repository { get; set; }
        public ICollection<WebHook> WebHook { get; set; }
    }
}
