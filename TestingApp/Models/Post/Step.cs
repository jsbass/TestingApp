using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestingApp.Models.Post
{
    public class Step
    {
        [JsonProperty("description")]
        public string description { get; set; }
    }
}
