using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestingApp.Models.DB
{
    [Table("Execution")]
    public class Execution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateExecuted { get; set; }

        [ForeignKey("Cycle")]
        public int CycleId { get; set; }

        public virtual Cycle Cycle { get; set; }

        public virtual ICollection<TestExecution> TestExecutions { get; set; }
    }
}
