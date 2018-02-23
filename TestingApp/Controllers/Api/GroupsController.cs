using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents.Linq;
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

            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                List<Models.Raven.Group> groups = new List<Models.Raven.Group>();
                if (includeFields.Any(f => f.HasNameAndFilter("tests", "")))
                {
                    groups = await db.Query<Models.Raven.Group>().Include(g => g.Tests).ToListAsync();
                }
                else
                {
                    groups = await db.Query<Models.Raven.Group>().ToListAsync();
                }

                return Ok(groups.Select(g => new Models.Outgoing.Group()
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Tests = g.Tests.Select(t =>
                    {
                        var test = db.LoadAsync<Models.Raven.Group>(t).Result;
                        return new Test()
                        {
                            Description = test.Description,
                            Name = test.Name,
                            Id = test.Id,
                            SerializeGroup = false
                        };
                    }).ToList(),
                    SerializeTests = includeFields.Any(f => f.HasNameAndFilter("tests", ""))
                }));
            }
        }
        
        // GET: api/Groups/5
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(string id, string include)
        {
            var includeFields = IncludeField.ParseList(include);

            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var group = await db.Include<Models.Raven.Group>(g => g.Tests).LoadAsync<Models.Raven.Group>(id);
                return Ok(new Models.Outgoing.Group()
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Tests = group.Tests.Select(t =>
                    {
                        var test = db.LoadAsync<Models.Raven.Group>(t).Result;
                        return new Test()
                        {
                            Description = test.Description,
                            Name = test.Name,
                            Id = test.Id,
                            SerializeGroup = false
                        };
                    }).ToList(),
                    SerializeTests = includeFields.Any(f => f.HasNameAndFilter("tests", ""))
                });
            }
        }

        // PUT: api/Groups/5
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] Group model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var group = await db.LoadAsync<Group>($"group/{id}");
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
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var group = new Models.Raven.Group()
                {
                    Description = model.Description,
                    Name = model.Name
                };

                await db.StoreAsync(group, null, null);
                await db.SaveChangesAsync();

                return CreatedAtAction("Get", new {id = group.Id}, group);
            }
        }

        // DELETE: api/Groups/5
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
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
        }

        #region ManagingChildren
        [HttpGet]
        [Route("{id}/tests")]
        public async Task<IActionResult> GetTests(string id)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var group = await db.Include<Models.Raven.Group>(g => g.Tests).LoadAsync<Models.Raven.Group>(id);
                if (group == null)
                {
                    return NotFound();
                }
                var tests = group.Tests.Select(async t => await db.LoadAsync<Models.Raven.Test>(t)).Select(t => t.Result).ToList();
                return Ok(tests.Select(t => new Models.Outgoing.Test()
                {
                    Description = t.Description,
                    Name = t.Name,
                    Id = t.Id,
                    SerializeGroup = false
                }));
            }
        }

        [HttpPut]
        [Route("{groupId}/tests/{testId}")]
        public async Task<IActionResult> AddTest(string groupId, string testId)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var group = await db.LoadAsync<Models.Raven.Group>(groupId);
                if (group == null)
                {
                    return NotFound(new {message = "group not found"});
                }
                if (!group.Tests.Contains(testId))
                {
                    var test = await db.LoadAsync<Models.Raven.Test>(testId);
                    if (test == null)
                    {
                        return NotFound(new { message = "test not found" });
                    }

                    group.Tests.Add(testId);
                }

                return NoContent();
            }
        }

        [HttpDelete]
        [Route("{groupId}/tests/{testId}")]
        public async Task<IActionResult> RemoveTest(string groupId, string testId)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var group = await db.LoadAsync<Models.Raven.Group>(groupId);
                if (group == null)
                {
                    return NotFound(new { message = "group not found" });
                }
                if (group.Tests.Contains(testId))
                {
                    var test = await db.LoadAsync<Models.Raven.Test>(testId);
                    if (test == null)
                    {
                        return NotFound(new { message = "test not found" });
                    }

                    group.Tests.Remove(testId);
                }

                return NoContent();
            }
        }


        #endregion
    }
}