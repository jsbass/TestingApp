using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestingApp.Models.Raven
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Parent { get; set; }
        public List<string> Tests { get; set; }
    }
}
