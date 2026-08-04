using Microsoft.AspNetCore.Mvc;
using QuranSchool.Api.Models;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Controllers;

[Route("api/[controller]")]
public class RequiredDocumentsController : BaseCrudController<RequiredDocument, IRequiredDocumentService>
{
    public RequiredDocumentsController(IRequiredDocumentService service) : base(service)
    {
    }

    [HttpGet("required-only")]
    public async Task<IActionResult> GetRequiredOnly() => Ok(await _service.GetRequiredOnlyAsync());
}
