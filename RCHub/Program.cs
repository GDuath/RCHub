using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RCHub.Data;
using RCHub.Mapping;
using RCHub.Services;

namespace RCHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Create database connection string from environment variables
            var db_server = builder.Configuration.GetValue<string>("DB_SERVER");
            var db_port = builder.Configuration.GetValue<string>("DB_PORT") ?? "3306";
            var db_cat = builder.Configuration.GetValue<string>("DB_CATALOGUE");
            var db_user = builder.Configuration.GetValue<string>("DB_USER");
            var db_pw = builder.Configuration.GetValue<string>("DB_PASSWORD");

            var connectionString = $"server={db_server};port={db_port};database={db_cat};user={db_user};password={db_pw};Convert Zero Datetime=True;";
            var serverVersion = new MariaDbServerVersion(new Version(11, 3, 0));

            // Add database context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseMySql(connectionString, serverVersion));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddDiscord(options =>
            {
                options.ClientId = builder.Configuration.GetValue<string>("DISCORD_CLIENTID") ?? string.Empty;
                options.ClientSecret = builder.Configuration.GetValue<string>("DISCORD_CLIENTSECRET") ?? string.Empty;
                options.CallbackPath = "/signin-discord";
            }).AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = false;
                options.AccessDeniedPath = "/Forbidden/";
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Authentication/Logout";
            });

            builder.Services.AddSingleton(typeof(DiscordBotService));
            builder.Services.AddSingleton(typeof(DynmapService));
            builder.Services.AddScoped(typeof(AppUserService));
            builder.Services.AddScoped(typeof(TownService));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddRazorPages();
            builder.Services.AddHttpContextAccessor();

            // Add controller mapping functions
            var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            var app = builder.Build();

            DbMigrationService.MigrationInitialization(app);


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                MinimumSameSitePolicy = SameSiteMode.Lax
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}