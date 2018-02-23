using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestingApp.Models.Outgoing
{
    public class Group
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tests")]
        public List<Test> Tests { get; set; }

        [JsonIgnore]
        public bool SerializeTests { get; set; }

        public bool ShouldSerializeTests()
        {
            return SerializeTests;
        }
    }
}
