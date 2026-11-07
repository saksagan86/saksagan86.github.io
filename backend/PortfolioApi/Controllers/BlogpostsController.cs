using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/blogposts")]
public class BlogpostsController : ControllerBase
{
    private readonly IBlogpostService _blogpostService;

    public BlogpostsController(IBlogpostService blogpostService)
    {
        _blogpostService = blogpostService;
    }

    // GET: api/blogposts
    [HttpGet]
    public async Task<ActionResult<List<BlogpostDto>>> GetAll()
    {
        var blogposts = await _blogpostService.GetAllAsync();
        return Ok(blogposts);
    }

    // GET: api/blogposts/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BlogpostDto>> GetById(int id)
    {
        var blogpost = await _blogpostService.GetByIdAsync(id);
        if (blogpost is null)
        {
            return NotFound();
        }

        return Ok(blogpost);
    }

    // POST: api/blogposts
    [HttpPost]
    public async Task<ActionResult<BlogpostDto>> Create(CreateBlogpostDto dto)
    {
        var blogpost = await _blogpostService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = blogpost.Id }, blogpost);
    }

    // PUT: api/blogposts/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BlogpostDto>> Update(int id, UpdateBlogpostDto dto)
    {
        var blogpost = await _blogpostService.UpdateAsync(id, dto);
        if (blogpost is null)
        {
            return NotFound();
        }

        return Ok(blogpost);
    }

    // DELETE: api/blogposts/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _blogpostService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}