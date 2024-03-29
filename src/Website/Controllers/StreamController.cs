using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Common;
using Website.Models.Database;

namespace Website.Controllers;

public class StreamController : Controller
{
    private readonly DatabaseContext _context;

    public StreamController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet("stream")]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Streams.OrderByDescending(x => x.Posted).Take(20).ToListAsync());
    }

    [HttpGet("stream/{id}")]
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var streamPost = await _context.Streams.FirstOrDefaultAsync(m => m.Id == id);
        if (streamPost == null)
        {
            return NotFound();
        }

        return View(streamPost);
    }

    [Authorize]
    [HttpGet("admin/stream/create")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost("admin/stream/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Content")] StreamPost streamPost)
    {
        if (ModelState.IsValid)
        {
            streamPost.Id = Guid.NewGuid();
            streamPost.Posted = DateTime.UtcNow;

            _context.Add(streamPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(streamPost);
    }

    [Authorize]
    [HttpGet("admin/stream/edit")]
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var streamPost = await _context.Streams.FindAsync(id);
        if (streamPost == null)
        {
            return NotFound();
        }
        return View(streamPost);
    }

    [Authorize]
    [HttpPost("admin/stream/edit")]
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
                _context.Update(streamPost);
                await _context.SaveChangesAsync();
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
    [HttpPost("admin/stream/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var streamPost = await _context.Streams.FirstOrDefaultAsync(m => m.Id == id);
        if (streamPost == null)
        {
            return NotFound();
        }


        _context.Streams.Remove(streamPost);
        await _context.SaveChangesAsync();

        Response.Htmx(headers =>
        {
            headers.Refresh();
        });

        return Ok();
    }

    private bool StreamPostExists(Guid id)
    {
        return _context.Streams.Any(e => e.Id == id);
    }
}
