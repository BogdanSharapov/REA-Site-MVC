using Microsoft.EntityFrameworkCore;

namespace REASite.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope()) 
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<REASiteDbContext>();
                dbContext.Database.Migrate();
            }

        }
    }
}
