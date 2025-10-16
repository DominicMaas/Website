using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Common;
using Website.Models.Database;

namespace Website.Controllers;

public class BlogController(DatabaseContext context) : Controller
{
    [HttpGet("blog")]
    public async Task<IActionResult> Index()
    {
        return View(await context.Streams.OrderByDescending(x => x.Posted).Take(20).ToListAsync());
    }

    [HttpGet("blog/{id}")]
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var streamPost = await context.Streams.FirstOrDefaultAsync(m => m.Id == id);
        if (streamPost == null)
        {
            return NotFound();
        }

        return View(streamPost);
    }

    [Authorize]
    [HttpGet("admin/blog/create")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost("admin/blog/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Content")] StreamPost streamPost)
    {
        if (ModelState.IsValid)
        {
            streamPost.Id = Guid.NewGuid();
            streamPost.Posted = DateTime.UtcNow;

            context.Add(streamPost);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(streamPost);
    }

    [Authorize]
    [HttpGet("admin/blog/edit")]
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var streamPost = await context.Streams.FindAsync(id);
        if (streamPost == null)
        {
            return NotFound();
        }
        return View(streamPost);
    }

    [Authorize]
    [HttpPost("admin/blog/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Title,Content")] StreamPost streamPost)
    {
        if (id != streamPost.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(streamPost);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StreamPostExists(streamPost.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(streamPost);
    }

    [Authorize]
    [HttpPost("admin/blog/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var streamPost = await context.Streams.FirstOrDefaultAsync(m => m.Id == id);
        if (streamPost == null)
        {
            return NotFound();
        }


        context.Streams.Remove(streamPost);
        await context.SaveChangesAsync();

        Response.Htmx(headers =>
        {
            headers.Refresh();
        });

        return Ok();
    }

    private bool StreamPostExists(Guid id)
    {
        return context.Streams.Any(e => e.Id == id);
    }
}
