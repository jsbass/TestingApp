using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestingApp.Models.Raven
{
    public class Cycle
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        class Execution
        {
            public string Name { get; set; }
            public List<TestExecution> Tests { get; set; }
        }

        class TestExecution
        {
            public string Id { get; set; }
            public string Status { get; set; }
            public DateTime? DateTimeExecuted { get; set; }
            public string Test { get; set; }
        }
    }
}
