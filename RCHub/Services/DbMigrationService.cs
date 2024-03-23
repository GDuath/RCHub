using Microsoft.EntityFrameworkCore;
using RCHub.Data;

namespace RCHub.Services
{
    public class DbMigrationService
    {
        public static void MigrationInitialization(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>()?.Database.Migrate();
            }
        }
    }
}
