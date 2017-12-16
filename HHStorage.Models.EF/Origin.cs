using System;
using System.Collections.Generic;

namespace HHStorage.Models.EF {
    public partial class Origin {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid? RepositoryId { get; set; }
        public string Origin1 { get; set; }

        public Repository Repository { get; set; }
        public User User { get; set; }
    }
}
