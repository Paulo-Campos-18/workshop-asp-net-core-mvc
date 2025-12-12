using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using SalesWebMvc.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace SalesWebMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<SalesWebMvcContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("SalesWebMvcContext"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SalesWebMvcContext"))
                )
            );

            builder.Services.AddScoped<SeedingService>();
            builder.Services.AddScoped<SellerService>();
            builder.Services.AddScoped<DepartmentService>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            var enUs = new CultureInfo("en-Us");
            var localizationIptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-Us"),
                SupportedCultures = new List<CultureInfo> { enUs},
                SupportedUICultures = new List<CultureInfo> { enUs},
            };
            app.UseRequestLocalization(localizationIptions);
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var seedingService = services.GetRequiredService<SeedingService>();
                    seedingService.Seed();
                }

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
