using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using packt_webapp.Services;

namespace packt_webapp.Middlewares
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();
        }

        public static async void AddSeedData(this IApplicationBuilder app)
        {
            using (var seedDataContext = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var seedDataService = seedDataContext.ServiceProvider.GetRequiredService<ISeedDataService>();
                await seedDataService.EnsureSeedData();
            }

            //2.0
            //var seedDavar seedDataContext = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            //var seedDataContext = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            //await seedDataService.EnsureSeedData();

            //1.1
            //var seedDataService = app.ApplicationServices.GetRequiredService<ISeedDataService>();
            //await seedDataService.EnsureSeedData();
        }
    }
}
