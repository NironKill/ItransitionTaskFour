using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserTable.Domain;

namespace UserTable.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<IdentityUserToken<Guid>> UserTokens { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);    
    }
}
