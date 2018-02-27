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
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("steps")]
        public List<Step> Steps { get; set; }
        
        [JsonIgnore]
        public bool SerializeSteps { get; set; }

        public bool ShouldSerializeSteps()
        {
            return SerializeSteps;
        }

        public class Step
        {
            [JsonProperty("description")]
            public string Description { get; set; }
        }
    }
}
