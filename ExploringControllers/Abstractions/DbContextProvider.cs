using Microsoft.EntityFrameworkCore;

namespace ExploringControllers
{
    public class DbContextProvider<TContext> : IDbContextProvider<TContext>
        where TContext : DbContext
    {

        public DbContextProvider(TContext context)
        {
            DbContext = context;
        }

        public TContext DbContext { get; set; }
    }
}
