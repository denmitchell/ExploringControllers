using Microsoft.EntityFrameworkCore;

namespace ExploringControllers
{
    public interface IDbContextProvider<TContext>
        where TContext : DbContext
    {
        TContext DbContext { get; set; }
    }
}
