using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HelloWorldFromDB
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
            string connectionString = @"Server=db;Database=master;User=sa;Password=Stubbs1234!; Trusted_Connection=False";
            //string connectionString = @"Server = (localdb)\mssqllocaldb; Database=LocalTest; Trusted_Connection=True; ";

            if (Configuration.GetConnectionString("HWDatabase") != null)
            {
                connectionString = Configuration.GetConnectionString("HWDatabase");
            }

            services.AddTransient<HelloWorldRepositoryContext>(
                serviceProvider => new HelloWorldRepositoryContextFactory().CreateDbContext(new string[]
                    {
                        connectionString
                    }
                )
            );
            services.AddSwaggerGen();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Game API");
                c.RoutePrefix = string.Empty;
            });
        }
    }

    public class HelloWorldRepositoryContextFactory : IDesignTimeDbContextFactory<HelloWorldRepositoryContext>
    {
        public HelloWorldRepositoryContext CreateDbContext(string[] args)
        {
            string connectionString = @"Server=db;Database=master;User=sa;Password=Stubbs1234!; Trusted_Connection=True";
            if (args.Length != 0)
            {
                connectionString = args[0];
            }
            var contextOptions = new DbContextOptionsBuilder<HelloWorldRepositoryContext>()
                .UseSqlServer(connectionString, options => { options.CommandTimeout(180); })
                .Options;
            return new HelloWorldRepositoryContext(contextOptions);
        }
    }
}
