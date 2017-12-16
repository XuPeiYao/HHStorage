using System;
using System.Collections.Generic;

namespace HHStorage.Models.EF {
    public partial class File {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid RepositoryId { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long? Size { get; set; }
        public string AccessModifier { get; set; }

        public Repository Repository { get; set; }
        public User User { get; set; }
    }
}
