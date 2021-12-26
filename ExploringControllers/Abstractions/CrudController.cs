using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExploringControllers
{

    /// <summary>
    /// Controller for performing Create, Read, Update, and Delete (CRUD)
    /// operations against a specific entity.  This controller inherits
    /// from IQueryController, which provides flexible querying capabilities.
    /// 
    /// </summary>
    /// <typeparam name="TContext">The DbContext of TEntity</typeparam>
    /// <typeparam name="TEntity">Target entity type</typeparam>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CrudController<TContext, TEntity> : Controller, ICrudController<TContext, TEntity> where TContext : DbContext
        where TEntity : class, new()
    {


        public TContext DbContext { get; set; }
        public ISysUserProvider SysUserProvider { get; set; }
        public ILogger Logger { get; set; }

        public CrudController(IDbContextProvider<TContext> dbContextProvider,
            ISysUserProvider userProvider,
            ILoggerFactory loggerFactory)
        {
            DbContext = dbContextProvider.DbContext;
            SysUserProvider = userProvider;
            Logger = loggerFactory.CreateLogger(GetType().Name);
        }


        #region Properties, Constants, and Abstract methods

        public const int UNPROCESSABLE_ENTITY_STATUS_CODE = 422;


        [NonAction]
        public abstract IQueryable<TEntity> Find(string pathParameter);


        #endregion

        #region Get

        [HttpGet]
        public virtual IActionResult GetAll()
        {
            return Ok(DbContext.Set<TEntity>()
                .AsNoTracking()
                .ToList());
        }


        [HttpGet("{**key}")]
        public virtual IActionResult GetById([FromRoute] string key)
        {
            key = WebUtility.UrlDecode(key);
            var qry = Find(key);

            var entity = qry.FirstOrDefault();

            if (entity == null)
                return NotFound();
            else
                return Ok(entity);
        }


        [HttpGet("async/{**key}")]
        public virtual async Task<IActionResult> GetByIdAsync([FromRoute] string key)
        {
            key = WebUtility.UrlDecode(key);
            var qry = Find(key);
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var entity = await qry.FirstOrDefaultAsync();

            if (entity == null)
                return NotFound();
            else
                return Ok(entity);
        }




        #endregion
        #region Create

        [HttpPost]
        public virtual IActionResult Create([FromBody] TEntity input)
        {

            BeforeCreate(input);
            DoCreate(input);

            return TrySave(input, input);
        }


        [HttpPost("async")]
        public virtual async Task<IActionResult> CreateAsync([FromBody] TEntity input)
        {

            BeforeCreate(input);
            DoCreate(input);

            return await TrySaveAsync(input, input);
        }

        #endregion
        #region Delete

        [HttpDelete("{**key}")]
        public virtual IActionResult Delete([FromRoute] string key)
        {
            key = WebUtility.UrlDecode(key);

            var qry = Find(key);
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = qry.FirstOrDefault();

            //check NotFound, Gone (deleted), Locked
            if (NotFound(existing, key, out IActionResult result))
                return result;

            BeforeDelete(existing);
            DoDelete(existing);

            return TrySave(existing, key);
        }


        [HttpDelete("async/{**key}")]
        public async virtual Task<IActionResult> DeleteAsync([FromRoute] string key)
        {
            key = WebUtility.UrlDecode(key);

            var qry = Find(key);
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = await qry.FirstOrDefaultAsync();

            //check NotFound, Gone (deleted), Locked
            if (NotFound(existing, key, out IActionResult result))
                return result;

            BeforeDelete(existing);
            DoDelete(existing);

            return await TrySaveAsync(existing, key);
        }

        #endregion
        #region Update

        [HttpPut("{**key}")]
        public virtual IActionResult Update([FromRoute] string key,
            [FromBody] TEntity input)
        {

            key = WebUtility.UrlDecode(key);

            //retrieve the existing entity
            var qry = Find(key);
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = qry.FirstOrDefault();

            //check NotFound, Gone (deleted), Locked
            if (NotFound(existing, key, out IActionResult result))
                return result;

            BeforeUpdate(existing);
            DoUpdate(input, existing);

            return TrySave(existing, input);

        }


        [HttpPut("async/{**key}")]
        public virtual async Task<IActionResult> UpdateAsync([FromRoute] string key,
            [FromBody] TEntity input)
        {
            key = WebUtility.UrlDecode(key);

            //retrieve the existing entity
            var qry = Find(key);
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = await qry.FirstOrDefaultAsync();

            //check NotFound, Gone (deleted), Locked
            if (NotFound(existing, key, out IActionResult result))
                return result;

            BeforeUpdate(existing);
            DoUpdate(input, existing);

            return await TrySaveAsync(existing, input);
        }

        #endregion
        #region Patch

        [HttpPatch("{**key}")]
        public virtual IActionResult Patch([FromRoute] string key,
            [FromBody] JsonElement input)
        {
            key = WebUtility.UrlDecode(key);

            if (input.ValueKind != JsonValueKind.Object)
                return new ObjectResult($"Cannot update {typeof(TEntity).Name} with {input.GetRawText().Substring(0, 200) + "..."}") { StatusCode = UNPROCESSABLE_ENTITY_STATUS_CODE };

            //retrieve the existing entity
            var qry = Find(key);
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = qry.FirstOrDefault();

            //check NotFound, Gone (deleted), Locked
            if (NotFound(existing, key, out IActionResult result))
                return result;

            BeforeUpdate(existing);
            DoPatch(input, existing);

            return TrySave(existing, input);
        }


        [HttpPatch("async/{**key}")]
        public virtual async Task<IActionResult> PatchAsync([FromRoute] string key,
            [FromBody] JsonElement input)
        {
            key = WebUtility.UrlDecode(key);

            if (input.ValueKind != JsonValueKind.Object)
                return new ObjectResult($"Cannot update {typeof(TEntity).Name} with {input.GetRawText().Substring(0, 200) + "..."}") { StatusCode = UNPROCESSABLE_ENTITY_STATUS_CODE };

            //retrieve the existing entity
            var qry = Find(key);
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = await qry.FirstOrDefaultAsync();

            //check NotFound, Gone (deleted), Locked
            if (NotFound(existing, key, out IActionResult result))
                return result;

            BeforeUpdate(existing);
            DoPatch(input, existing);

            return await TrySaveAsync(existing, input);

        }

        #endregion
        #region HelperMethods

        private bool NotFound(TEntity entity, string key, out IActionResult result)
        {

            result = null;
            if (entity == null)
            {
                Logger.LogWarning("{Entity} record could not be found for key {Key}", typeof(TEntity).Name, key);
                ModelState.AddModelError("", $"The {typeof(TEntity).Name} record could not be found for key: {key}");
                result = NotFound(ModelState);
                return true;
            }
            else
                return false;
        }


        #endregion
        #region Lifecycle Methods

        [NonAction]
        public virtual void BeforeCreate(TEntity input) { }

        [NonAction]
        public virtual void BeforeUpdate(TEntity existing) { }

        [NonAction]
        public virtual void BeforeDelete(TEntity existing) { }

        [NonAction]
        public virtual void DoCreate(TEntity input)
            => DbContext.Add(input);

        [NonAction]
        public virtual void DoDelete(TEntity existing)
            => DbContext.Remove(existing);

        [NonAction]
        public virtual void DoUpdate(TEntity input, TEntity existing)
        {
            foreach (var prop in existing.GetType().GetProperties())
            {
                prop.SetValue(existing, prop.GetValue(input));
            }
        }

        [NonAction]
        public virtual void DoPatch(JsonElement input, TEntity existing)
        {
            foreach (var propName in _propertyDict.Keys)
            {
                if (input.TryGetProperty(propName, out JsonElement jel))
                {
                    var value = JsonSerializer.Deserialize(jel.GetRawText(),
                        _propertyDict[propName].PropertyType, _jsonSerializerOptions);
                    _propertyDict[propName].SetValue(existing, value);
                }
            }
        }

        private static readonly Dictionary<string, PropertyInfo> _propertyDict;

        private static readonly JsonSerializerOptions _jsonSerializerOptions
            = new()
            { PropertyNameCaseInsensitive = true };


        static CrudController()
        {
            var properties = typeof(TEntity).GetProperties();
            _propertyDict = new Dictionary<string, PropertyInfo>();

            for (int i = 0; i < properties.Length; i++)
            {

                //PascalCase
                _propertyDict.Add(properties[i].Name, properties[i]);

                //camelCase
                _propertyDict.Add(properties[i].Name.Substring(0, 1).ToLower()
                    + properties[i].Name[1..], properties[i]);
            }
        }


        #endregion

        #region Helper Methods


        private IQueryable<TEntity> GetQuery(bool asNoTracking = true)
        {
            var dbSet = DbContext
                .Set<TEntity>();

            IQueryable<TEntity> qry = dbSet.AsQueryable();

            if (asNoTracking)
                qry = qry
                .AsNoTracking();

            return qry;
        }


        private void UpdateSysUser()
        {
            var sysUser = SysUserProvider.SysUser;

            var entries = DbContext.ChangeTracker.Entries().ToList();
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry.Entity is IHasSysUser entity)
                    switch (entry.State)
                    {
                        case EntityState.Added:
                        case EntityState.Modified:
                        case EntityState.Deleted:
                            entity.SysUser = sysUser;
                            break;
                        default:
                            break;
                    }
            }
        }

        private IActionResult TrySave(object objectToReturn, object objectToLog)
        {
            try
            {
                UpdateSysUser();
                DbContext.ChangeTracker.DetectChanges();
                DbContext.SaveChanges();
                return Ok(objectToReturn);
            }
            catch (Exception ex)
            {
                return HandleSaveException(ex, objectToLog);
            }
        }


        private async Task<IActionResult> TrySaveAsync(object objectToReturn,
            object objectToLog)
        {
            try
            {
                UpdateSysUser();
                DbContext.ChangeTracker.DetectChanges();
                await DbContext.SaveChangesAsync();
                return Ok(objectToReturn);
            }
            catch (Exception ex)
            {
                return HandleSaveException(ex, objectToLog);
            }
        }


        private ObjectResult HandleSaveException(Exception ex, object objectToLog)
        {
            var errObj = new { Exception = ex.Message, Object = objectToLog };
            if (ex is DbUpdateException)
            {
                return new ObjectResult(errObj) { StatusCode = (int)HttpStatusCode.Conflict };
            }
            else
            {
                return new ObjectResult(errObj) { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

        }



        #endregion

    }
}
