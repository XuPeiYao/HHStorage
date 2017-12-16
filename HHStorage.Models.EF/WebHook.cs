using System;
using System.Collections.Generic;

namespace HHStorage.Models.EF {
    public partial class WebHook {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid? RepositoryId { get; set; }
        public string Url { get; set; }

        public Repository Repository { get; set; }
        public User User { get; set; }
    }
}
