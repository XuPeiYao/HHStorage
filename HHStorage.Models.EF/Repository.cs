using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public ICollection<File> File { get; set; }

        [JsonIgnore]
        public ICollection<Origin> Origin { get; set; }

        [JsonIgnore]
        public ICollection<WebHook> WebHook { get; set; }
    }
}
