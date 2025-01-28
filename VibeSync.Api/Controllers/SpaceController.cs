using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Requests;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpaceController(ISpaceUseCase useCase) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpaceById(Guid id)
    {
        Debug.WriteLine("GetSpaceById called with id: " + id);
        var space = await useCase.GetSpaceById(id);
        return Ok(space);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpace([FromBody] CreateSpaceRequest request)
    {
        var space = await useCase.CreateSpace(request);
        return CreatedAtAction(nameof(GetSpaceById), new { id = space.Id }, space);
    }
}