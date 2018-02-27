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
        public List<Execution> Executions { get; set; }

        public Cycle()
        {
            Executions = new List<Execution>();
        }

        public class Execution
        {
            public string Name { get; set; }
            public List<TestExecution> Tests { get; set; }

            public Execution()
            {
                Tests = new List<TestExecution>();
            }
        }

        public class TestExecution
        {
            public string Id { get; set; }
            public string Status { get; set; }
            public DateTime? DateTimeExecuted { get; set; }
            public string Test { get; set; }
        }
    }
}
