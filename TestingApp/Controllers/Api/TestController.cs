using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using TestingApp.Models;
using TestingApp.Models.Raven;

namespace TestingApp.Controllers.Api
{
    [Route("api/tests")]
    public class TestController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var tests = await db.Query<Test>().ToListAsync();

                return Ok(tests.Select(t => new Models.Outgoing.Test()
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Steps = t.Steps.Select(s => new Models.Outgoing.Test.Step()
                    {
                        Description = s.Description
                    }).ToList(),
                    SerializeSteps = true
                }));
            }
        }

        // GET: api/Tests/5
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var test = await db.LoadAsync<Models.Raven.Test>($"tests/{id}");
                if (test == null)
                {
                    return NotFound();
                }

                return Ok(new Models.Outgoing.Test()
                {
                    Id = test.Id,
                    Name = test.Name,
                    Description = test.Description,
                    Steps = test.Steps.Select(s => new Models.Outgoing.Test.Step()
                    {
                        Description = s.Description
                    }).ToList(),
                    SerializeSteps = true
                });
            }
        }

        // PUT: api/Tests/5'
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] Models.Post.Test model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var test = await db.LoadAsync<Models.Raven.Test>($"tests/{id}");
                if (test == null)
                {
                    return NotFound();
                }

                test.Description = model.Description;
                test.Name = model.Name;

                await db.SaveChangesAsync();

                return NoContent();
            }
        }

        // POST: api/Tests
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Models.Post.Test model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var test = new Models.Raven.Test()
                {
                    Description = model.Description,
                    Name = model.Name
                };

                await db.StoreAsync(test, null, null);
                await db.SaveChangesAsync();

                return CreatedAtAction("Get", new {id = test.Id}, test);
            }
        }

        // DELETE: api/Tests/5
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
                db.Delete($"tests/{id}");
                await db.SaveChangesAsync();
                return NoContent();
            }
        }

        #region ManagingSteps

        [HttpGet]
        [Route("{id}/steps")]
        public async Task<IActionResult> GetSteps(string id)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var test = await db.LoadAsync<Models.Raven.Test>($"tests/{id}");
                if (test == null)
                {
                    return NotFound();
                }

                return Ok(test.Steps.Select(t => new Models.Outgoing.Test.Step()
                {
                    Description = t.Description
                }));
            }
        }

        [HttpPost]
        [Route("{id}/steps")]
        public async Task<IActionResult> AddStep(string id, [FromBody] Models.Post.Step model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var test = await db.LoadAsync<Models.Raven.Test>($"tests/{id}");
                if (test == null)
                {
                    return NotFound();
                }

                test.Steps.Add(new Models.Raven.Test.Step()
                {
                    Description = model.description
                });

                await db.SaveChangesAsync();

                return NoContent();
            }
        }

        [HttpPost]
        [Route("{id}/steps/{at}")]
        public async Task<IActionResult> AddStepAt(string id, int at, [FromBody] Models.Post.Step model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var test = await db.LoadAsync<Models.Raven.Test>($"tests/{id}");
                if (test == null)
                {
                    return NotFound();
                }

                try
                {
                    test.Steps.Insert(at, new Models.Raven.Test.Step()
                    {
                        Description = model.description
                    });
                }
                catch (ArgumentOutOfRangeException e)
                {
                    return BadRequest(new { message = "at index is out of range of the list of steps"});
                }

                await db.SaveChangesAsync();

                return NoContent();
            }
        }

        [HttpDelete]
        [Route("{id}/steps/{at}")]
        public async Task<IActionResult> RemoveStep(string id, int at)
        {
            using (var db = RavenStore.Store.OpenAsyncSession())
            {
                var test = await db.LoadAsync<Models.Raven.Test>($"tests/{id}");
                if (test == null)
                {
                    return NotFound(new { message = "group not found" });
                }
                
                test.Steps.RemoveAt(at);
                await db.SaveChangesAsync();

                return NoContent();
            }
        }
        #endregion
    }
}
