using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using TestingApp.Helpers;
using TestingApp.Models;
using TestingApp.Models.DB;
using TestingApp.Models.Raven;
using Group = TestingApp.Models.Outgoing.Group;
using Test = TestingApp.Models.Outgoing.Test;

namespace TestingApp.Controllers.Api
{
    //TODO add group child parent relationship
    [Produces("application/json")]
    [Route("api/v1/groups")]
    public class GroupsController : Controller
    {
        [HttpGet]
        [Route("tree/v1")]
        public async Task<IActionResult> GetTree()
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var list = (await db.Query<Models.Raven.Group>().ToListAsync()).GenerateTree(g => g.Id, g => g.Parent);
                return Ok(ConvertTreeToGroupHierarchy(list));
            }
        }

        private List<Group> ConvertTreeToGroupHierarchy(IEnumerable<TreeItem<Models.Raven.Group>> root)
        {
            return root.Select(i => new Group()
            {
                Name = i.Item.Name,
                Description = i.Item.Description,
                Id = i.Item.Description,
                SerializeHeirarchy = true,
                SerializeTests = false,
                Children = ConvertTreeToGroupHierarchy(i.Children)
            }).ToList();
        }

        //Todo maybe turn into node tree
        // GET: api/Groups
        [HttpGet]
        public async Task<IActionResult> Get(string include)
        {
            var includeFields = IncludeField.ParseList(include);

            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                List<Models.Raven.Group> groups = new List<Models.Raven.Group>();
                var includeTests = includeFields.Any(f => f.HasNameAndFilter("tests", ""));
                if (includeTests)
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
                    Tests = includeTests ? g.Tests.Select(t =>
                    {
                        var test = db.LoadAsync<Models.Raven.Group>(t).Result;
                        return new Models.Outgoing.Test()
                        {
                            Description = test.Description,
                            Name = test.Name,
                            Id = test.Id,
                            SerializeSteps = false
                        };
                    }).ToList() : null,
                    SerializeTests = includeTests
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
                var includeTests = includeFields.Any(f => f.HasNameAndFilter("tests", ""));
                Models.Raven.Group group = null;
                if (includeTests)
                {
                    group = await db.Include<Models.Raven.Group>(g => g.Tests).LoadAsync<Models.Raven.Group>($"groups/{id}");
                }
                else
                {
                    group = await db.LoadAsync<Models.Raven.Group>($"groups/{id}");
                }
                if (group == null)
                {
                    return NotFound();
                }

                return Ok(new Models.Outgoing.Group()
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Tests = includeTests ? group.Tests.Select(t =>
                    {
                        var test = db.LoadAsync<Models.Raven.Group>(t).Result;
                        return new Models.Outgoing.Test()
                        {
                            Description = test.Description,
                            Name = test.Name,
                            Id = test.Id,
                            SerializeSteps = false
                        };
                    }).ToList() : null,
                    SerializeTests = includeTests
                });
            }
        }

        // PUT: api/Groups/5
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] Models.Post.Group model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var group = await db.LoadAsync<Models.Raven.Group>($"groups/{id}");
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
        public async Task<IActionResult> Create([FromBody] Models.Post.Group model)
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
                var group = await db.Include<Models.Raven.Group>(g => g.Tests).LoadAsync<Models.Raven.Group>($"groups/{id}");
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
                    SerializeSteps = false
                }));
            }
        }

        [HttpPut]
        [Route("{groupId}/tests/{testId}")]
        public async Task<IActionResult> AddTest(string groupId, string testId)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                testId = $"tests/{testId}";
                var group = await db.LoadAsync<Models.Raven.Group>($"groups/{groupId}");
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
                    await db.SaveChangesAsync();
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
                testId = $"tests/{testId}";
                var group = await db.LoadAsync<Models.Raven.Group>($"groups/{groupId}");
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
                    await db.SaveChangesAsync();
                }

                return NoContent();
            }
        }

        [HttpGet]
        [Route("{id}/children")]
        public async Task<IActionResult> GetChildren(string id)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var children = await db.Query<Models.Raven.Group>().Where(g => g.Parent == $"groups/{id}").ToListAsync();
                return Ok(children.Select(t => new Models.Outgoing.Group()
                {
                    Description = t.Description,
                    Name = t.Name,
                    Id = t.Id,
                    SerializeHeirarchy = false,
                    SerializeTests = false
                }));
            }
        }

        [HttpGet]
        [Route("{id}/children/tree")]
        public async Task<IActionResult> GetChildrenTree(string id)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var group = await db.LoadAsync<Models.Raven.Group>($"groups/{id}");
                if (group == null)
                {
                    return NotFound();
                }
                var list = (await db.Query<Models.Raven.Group>().ToListAsync()).GenerateTree(g => g.Id, g => g.Parent, $"groups/{id}");
                return Ok(ConvertTreeToGroupHierarchy(list));
            }
        }

        [HttpPut]
        [Route("{parentId}/children/{childId}")]
        public async Task<IActionResult> AddChild(string parentId, string childId)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var child = await db.LoadAsync<Models.Raven.Group>($"groups/{childId}");
                if (child == null)
                {
                    return NotFound(new {message = "child group not found"});
                }
                if (child.Parent != parentId)
                {
                    var parent = await db.LoadAsync<Models.Raven.Test>($"groups/{parentId}");
                    if (parent == null)
                    {
                        return NotFound(new { message = "parent group not found" });
                    }

                    child.Parent = parent.Id;
                    await db.SaveChangesAsync();
                }

                return NoContent();
            }
        }

        [HttpDelete]
        [Route("{parentId}/children/{childId}")]
        public async Task<IActionResult> RemoveChild(string parentId, string childId)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var child = await db.LoadAsync<Models.Raven.Group>($"groups/{childId}");
                if (child == null)
                {
                    return NotFound(new { message = "child group not found" });
                }
                if (child.Parent != parentId)
                {
                    var parent = await db.LoadAsync<Models.Raven.Test>($"groups/{parentId}");
                    if (parent == null)
                    {
                        return NotFound(new { message = "parent group not found" });
                    }

                    child.Parent = null;
                    await db.SaveChangesAsync();
                }

                return NoContent();
            }
        }


        #endregion
    }
}