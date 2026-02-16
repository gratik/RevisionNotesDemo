using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using RevisionNotes.Microservice.CatalogService.Features.Catalog;

namespace RevisionNotes.Microservice.CatalogService.Controllers;

[ApiController]
[Route("api/catalog")]
public sealed class CatalogController(
    ICatalogRepository repository,
    IOutputCacheStore outputCacheStore) : ControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 20, Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CatalogItemResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await repository.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CatalogItemResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "catalog.write")]
    [EnableRateLimiting("write-policy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CatalogItemResponse>> Create([FromBody] CreateCatalogItemRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name) || request.Price <= 0)
        {
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["request"] = ["Sku/Name are required and price must be > 0"]
            }));
        }

        var created = await repository.CreateAsync(request, cancellationToken);
        await outputCacheStore.EvictByTagAsync("catalog", cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
