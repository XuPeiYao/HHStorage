using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HHStorage.Models.EF {
    public partial class File {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public Guid RepositoryId { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long? Size { get; set; }

        [JsonIgnore]
        public string accessModifierString { get; set; }

        public AccessModifierTypes AccessModifier {
            get {
                return Enum.Parse<AccessModifierTypes>(accessModifierString);
            }
            set {
                accessModifierString = value.ToString();
            }
        }

        public Repository Repository { get; set; }
        public User User { get; set; }
    }
}
