using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestingApp.Models.DB
{
    public class TestingContext : DbContext
    {
        private const string ConnectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Testing;Persist Security Info=True;User ID=testingaccount;Password=Testing123";

        public TestingContext() : base((new DbContextOptionsBuilder<TestingContext>().UseSqlServer(ConnectionString)).Options)
        { }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Execution> Executions { get; set; }
        public DbSet<TestExecution> TestExecutions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TestExecution>().HasKey(c => new {c.ExecutionId, c.TestId});
        }
    }
}
