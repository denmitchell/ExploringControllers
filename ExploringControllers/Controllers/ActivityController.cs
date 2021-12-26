using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ExploringControllers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : CrudController<AppDbContext, Activity>
    {

        [ActivatorUtilitiesConstructor]
        public ActivityController(IDbContextProvider<AppDbContext> dbContextProvider, 
            ISysUserProvider userProvider, ILoggerFactory loggerFactory) 
            : base(dbContextProvider, userProvider, loggerFactory)
        {
        }


        public override IQueryable<Activity> Find(string pathParameter)
            => DbContext.Activities.Where(a=>a.Id == int.Parse(pathParameter));
    }
}
