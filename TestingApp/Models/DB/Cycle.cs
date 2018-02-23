using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestingApp.Models.DB
{
    public class Cycle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Execution> Executions { get; set; }
    }
}
