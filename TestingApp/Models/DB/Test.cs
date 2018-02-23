using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TestingApp.Models.DB
{
    [Table("Test")]
    public class Test
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        public virtual Group Group { get; set; }

        public virtual ICollection<TestExecution> TestExecutions { get; set; }
    }
}
