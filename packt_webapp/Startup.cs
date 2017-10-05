using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using packt_webapp.Middlewares;
using Microsoft.EntityFrameworkCore;
using packt_webapp.Entities;
using packt_webapp.Repositories;
using packt_webapp.Dtos;
using packt_webapp.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using NLog.Web;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
//using Microsoft.AspNetCore.Mvc.Versioning;
//using IdentityServer4.AccessTokenValidation;

namespace packt_webapp
{
    //public class MyConfiguration
    //    public string Firstname { get; set; }
    //    public string Lastname { get; set; }
    //}



    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            env.ConfigureNLog("nlog.config");

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

            //services.AddDbContext<PacktDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContextPool<PacktDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<ISeedDataService, SeedDataService>();

            services.AddSwaggerGen( config=> {
                config.SwaggerDoc("v1", new Info { Title = "My WebAPI", Version = "v1"});
            });

            services.AddMvc(config =>
            {
                config.ReturnHttpNotAcceptable = true;
                config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                config.InputFormatters.Add(new XmlSerializerInputFormatter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Error);

            //nlog
            loggerFactory.AddNLog();
            // add NLog Web
            app.AddNLogWeb();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (errorFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500, errorFeature.Error, errorFeature.Error.Message);
                        }

                        await context.Response.WriteAsync("There was an error, Bunky");
                    });
                });
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

			app.UseSwagger();
			app.UseSwaggerUI(config =>
			{
				config.SwaggerEndpoint("/swagger/v1/swagger.json", "My WebAPI");
			});


            AutoMapper.Mapper.Initialize(mapper =>
            {
                mapper.CreateMap<Customer, CustomerDto>().ReverseMap();
                mapper.CreateMap<Customer, CustomerCreateDto>().ReverseMap();
                mapper.CreateMap<Customer, CustomerUpdateDto>().ReverseMap();
            });


            //app.UseMiddleware<CustomMiddleware>();
            //app.UseCustomMiddleware();

            app.AddSeedData();

            app.UseMvc();
        }
    }
}
