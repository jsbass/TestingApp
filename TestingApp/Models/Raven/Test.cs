using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestingApp.Models.Raven
{
    public class Test
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Step> Steps { get; set; }

        public Test()
        {
            Steps = new List<Step>();
        }

        public class Step
        {
            public string Description { get; set; }
        }
    }
}
