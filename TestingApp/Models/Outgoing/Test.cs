using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestingApp.Models.Outgoing
{
    public class Test
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("group")]
        public Group Group { get; set; }
        
        [JsonIgnore]
        public bool SerializeGroup { get; set; }

        public bool ShouldSerializeGroup()
        {
            return SerializeGroup;
        }
    }
}
