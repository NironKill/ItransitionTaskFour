using Microsoft.EntityFrameworkCore;

namespace UserTable.Persistence
{
    public static class Preparation
    {
        public static async Task Initialize(ApplicationDbContext context) => await context.Database.MigrateAsync();
    }
}
