using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestingApp.Models.DB
{
    [Table("TestExecution")]
    public class TestExecution
    {
        [ForeignKey("Test")]
        public int TestId { get; set; }
        
        [ForeignKey("Execution")]
        public long ExecutionId { get; set; }

        public virtual Test Test { get; set; }
        public virtual Execution Execution { get; set; }
    }
}
