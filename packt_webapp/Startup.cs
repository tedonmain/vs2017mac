using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using packt_webapp.Middlewares;
using System.IO;
using System.Diagnostics;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer;
using Microsoft.Data;
using Microsoft.EntityFrameworkCore;
using packt_webapp.Entities;
using packt_webapp.Repositories;
using packt_webapp.Dtos;
using packt_webapp.Services;

namespace packt_webapp
{
    //public class MyConfiguration
    //    public string Firstname { get; set; }
    //    public string Lastname { get; set; }
    //}



    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env) {
			var builder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
				.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            //Debug.WriteLine($" ---> from config: {Configuration["firstname"]}");
            //Debug.WriteLine($" ---> from config: {Configuration["lastname"]}");

        }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            //services.Configure<MyConfiguration>(Configuration);

            services.AddDbContext<PacktDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<ISeedDataService, SeedDataService>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (env.IsEnvironment("MyEnvironment"))
			{
				app.UseCustomMiddleware();
            }

            // use index.html instead
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.UseDefaultFiles();
            app.UseStaticFiles();


            AutoMapper.Mapper.Initialize(mapper => { mapper.CreateMap<Customer, CustomerDto>().ReverseMap(); });

            //app.UseMiddleware<CustomMiddleware>();
            //app.UseCustomMiddleware();

            // 
            //app.AddSeedData();

            app.UseMvc();
        }
    }
}
