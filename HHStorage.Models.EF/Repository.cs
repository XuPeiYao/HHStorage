using System;
using System.Collections.Generic;

namespace HHStorage.Models.EF {
    public partial class Repository {
        public Repository() {
            File = new HashSet<File>();
            Origin = new HashSet<Origin>();
            WebHook = new HashSet<WebHook>();
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string UserId { get; set; }
        public string AccessModifier { get; set; }

        public User User { get; set; }
        public ICollection<File> File { get; set; }
        public ICollection<Origin> Origin { get; set; }
        public ICollection<WebHook> WebHook { get; set; }
    }
}
