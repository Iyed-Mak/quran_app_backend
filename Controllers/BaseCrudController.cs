using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[ApiController]
[Authorize]
public abstract class BaseCrudController<TEntity, TService>(TService service) : ControllerBase
    where TEntity : class, IEntity
    where TService : IService<TEntity>
{
    protected readonly TService _service = service;

    [HttpGet]
    public virtual async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public virtual async Task<IActionResult> GetById(int id)
    {
        var entity = await _service.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Create([FromBody] TEntity entity)
    {
        // تجاهل معرّف قد يُرسله العميل مع الطلب: المفاتيح الأساسية
        // مولّدة من قاعدة البيانات، وتمرير قيمة مثل -1 كان يُدرج صفًا
        // بمعرّف سالب ثم يفشل الحفظ التالي بتكرار المفتاح الأساسي.
        entity.Id = default;
        var created = await _service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public virtual async Task<IActionResult> Update(int id, [FromBody] TEntity entity)
    {
        if (id != entity.Id)
        {
            return BadRequest("Route id does not match body id.");
        }

        await _service.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public virtual async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
