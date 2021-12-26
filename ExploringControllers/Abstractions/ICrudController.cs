using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExploringControllers
{
    public interface ICrudController<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class, new()
    {
        TContext DbContext { get; set; }
        ILogger Logger { get; set; }
        ISysUserProvider SysUserProvider { get; set; }

        void BeforeCreate(TEntity input);
        void BeforeDelete(TEntity existing);
        void BeforeUpdate(TEntity existing);
        IActionResult Create([FromBody] TEntity input);
        Task<IActionResult> CreateAsync([FromBody] TEntity input);
        IActionResult Delete([FromRoute] string key);
        Task<IActionResult> DeleteAsync([FromRoute] string key);
        void DoCreate(TEntity input);
        void DoDelete(TEntity existing);
        void DoPatch(JsonElement input, TEntity existing);
        void DoUpdate(TEntity input, TEntity existing);
        IQueryable<TEntity> Find(string pathParameter);
        IActionResult GetAll();
        IActionResult GetById([FromRoute] string key);
        Task<IActionResult> GetByIdAsync([FromRoute] string key);
        IActionResult Patch([FromRoute] string key, [FromBody] JsonElement input);
        Task<IActionResult> PatchAsync([FromRoute] string key, [FromBody] JsonElement input);
        IActionResult Update([FromRoute] string key, [FromBody] TEntity input);
        Task<IActionResult> UpdateAsync([FromRoute] string key, [FromBody] TEntity input);
    }
}