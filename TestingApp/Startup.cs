using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using TestingApp.Models.DB;

namespace TestingApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info()
                {
                    Title = "Testing Api",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSwagger(options =>
            {
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Testing Api V1");
            });
            
            app.UseMvc(routes =>
            {
                var Api404Handler = new RouteHandler(context =>
                {
                    context.Response.StatusCode = HttpStatusCode.NotFound.GetHashCode();
                    return Task.CompletedTask;
                });
                routes.Routes.Add(new Route(Api404Handler, "api/{*url}", defaults: null, constraints: null, dataTokens: null, inlineConstraintResolver: routes.ApplicationBuilder.ApplicationServices.GetRequiredService<IInlineConstraintResolver>()));
                routes.MapRoute("AngularRoute", "{*url}", new { controller = "Angular", action = "Index" });
            });
        }
    }
}
