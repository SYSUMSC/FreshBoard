using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mscfreshman.Data.Identity;

namespace mscfreshman.Data
{
    public class ApplicationDbContext : IdentityDbContext<FreshBoardUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
