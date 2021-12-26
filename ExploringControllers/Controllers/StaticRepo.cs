using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExploringControllers.Controllers
{
    public static class StaticRepo
    {
        public static IEnumerable<TEntity> GetPage<TEntity>(AppDbContext dbContext, int skip, int take)
            where TEntity : class
        {
            return dbContext.Set<TEntity>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take);
        }
    }
}
