using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingApp.Models;
using TestingApp.Models.DB;
using TestingApp.Models.Raven;
using Group = TestingApp.Models.Post.Group;
using Test = TestingApp.Models.Outgoing.Test;

namespace TestingApp.Controllers.Api
{
    //TODO add group child parent relationship
    [Produces("application/json")]
    [Route("api/groups")]
    public class GroupsController : Controller
    {
        //Todo maybe turn into node tree
        // GET: api/Groups
        [HttpGet]
        public async Task<IActionResult> Get(string include)
        {
            var includeFields = IncludeField.ParseList(include);

            using (var db = new TestingContext())
            {
                return Ok(await db.Groups.Select(g => new Models.Outgoing.Group()
                {
                    Description = g.Description,
                    Id = g.Id,
                    Name = g.Name,
                    Tests = g.Tests.Select(t => new Models.Outgoing.Test()
                    {
                        Description = t.Description,
                        Id = t.Id,
                        Name = t.Name,
                        SerializeGroup = false
                    }).ToList(),
                    SerializeTests = includeFields.Any(f => f.HasNameAndFilter("tests", ""))
                }).ToListAsync());
            }
        }
        
        // GET: api/Groups/5
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id, string include)
        {
            var includeFields = IncludeField.ParseList(include);
            using (var db = new TestingContext())
            {
                var group = await db.Groups.Include(g => g.Tests).SingleOrDefaultAsync(g => g.Id == id);
                if (group == null)
                {
                    return NotFound();
                }
                return Ok(new Models.Outgoing.Group()
                {
                    Description = group.Description,
                    Id = group.Id,
                    Name = group.Name,
                    Tests = group.Tests.Select(t => new Models.Outgoing.Test()
                    {
                        Description = t.Description,
                        Id = t.Id,
                        Name = t.Name,
                        SerializeGroup = false
                    }).ToList(),
                    SerializeTests = includeFields.Any(f => f.HasNameAndFilter("tests", ""))
                });
            }
        }

        // PUT: api/Groups/5
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Group model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var db = new TestingContext())
            {
                var group = await db.Groups.SingleOrDefaultAsync(g => g.Id == id);
                if (group == null)
                {
                    return NotFound();
                }

                group.Description = model.Description;
                group.Name = model.Name;
                
                await db.SaveChangesAsync();

                return NoContent();
            }
        }

        // POST: api/Groups
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Group model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var db = new TestingContext())
            {
                var group = new Models.DB.Group()
                {
                    Name = model.Name,
                    Description = model.Description
                };

                db.Groups.Add(group);
                await db.SaveChangesAsync();

                return CreatedAtAction("Get", new {id = group.Id}, group);
            }
        }

        // DELETE: api/Groups/5
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                db.Delete($"groups/{id}");
                await db.SaveChangesAsync();
                return NoContent();
            }
            using (var db = new TestingContext())
            {
                var group = await db.Groups.SingleOrDefaultAsync(m => m.Id == id);
                if (group == null)
                {
                    return NotFound();
                }

                db.Groups.Remove(group);
                await db.SaveChangesAsync();

                return Ok(group);
            }
        }

        #region ManagingChildren
        [HttpGet]
        [Route("{id}/tests")]
        public async Task<IActionResult> GetTests(int id)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
            }

            using (var db = new TestingContext())
            {
                var group = await db.Groups.Include(g => g.Tests).SingleOrDefaultAsync(g => g.Id == id);
                if (group == null)
                {
                    return NotFound();
                }

                return Ok(group.Tests.Select(t => new Models.Outgoing.Test()
                {
                    Description = t.Description,
                    Id = t.Id,
                    Name = t.Name,
                    SerializeGroup = false
                }));
            }
        }

        [HttpPut]
        [Route("{groupId}/tests/{testId}")]
        public async Task<IActionResult> AddTest(int groupId, int testId)
        {
            using (var db = new TestingContext())
            {
                var group = await db.Groups.Include(g => g.Tests).SingleOrDefaultAsync(g => g.Id == groupId);
                var test = await db.Tests.SingleOrDefaultAsync(t => t.Id == testId);
                if (group == null)
                {
                    return NotFound(new { message = "group not found"});
                }

                if (test == null)
                {
                    return NotFound(new { message = "test not found"});
                }

                group.Tests.Add(test);

                await db.SaveChangesAsync();

                return NoContent();
            }
        }

        [HttpDelete]
        [Route("{groupId}/tests/{testId}")]
        public async Task<IActionResult> RemoveTest(int groupId, int testId)
        {
            using (var db = new TestingContext())
            {
                var group = await db.Groups.Include(g => g.Tests).SingleOrDefaultAsync(g => g.Id == groupId);
                var test = await db.Tests.SingleOrDefaultAsync(t => t.Id == testId);
                if (group == null)
                {
                    return NotFound(new { message = "group not found" });
                }

                if (test == null)
                {
                    return NotFound(new { message = "test not found" });
                }

                group.Tests.Remove(test);

                await db.SaveChangesAsync();

                return NoContent();
            }
        }


        #endregion
    }
}