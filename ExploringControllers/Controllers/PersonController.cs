using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;


namespace ExploringControllers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : CrudController<AppDbContext, Person>
    {
        public PersonController(IDbContextProvider<AppDbContext> dbContextProvider,
            ISysUserProvider userProvider, ILoggerFactory loggerFactory)
            : base(dbContextProvider, userProvider, loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        ILoggerFactory _loggerFactory;

        public override IQueryable<Person> Find(string pathParameter)
            => DbContext.Persons.Where(a => a.Id == int.Parse(pathParameter));


        [HttpGet("activities/instantiated")]
        public IActionResult GetActivities()
        {
            var activityController = new ActivityController(
                new DbContextProvider<AppDbContext>(DbContext),
                SysUserProvider = SysUserProvider,
                _loggerFactory
                );
            return activityController.GetAll();
        }


        [HttpGet("activities/injected")]
        public IActionResult GetActivities([FromServices] ICrudController<AppDbContext, Activity> activityController)
        {
            return activityController.GetAll();
        }


        [HttpGet("activities/static-repo")]
        public IActionResult GetActivities(int skip, int take)
        {
            return Ok(StaticRepo.GetPage<Activity>(DbContext,1,1));
        }


    }
}
