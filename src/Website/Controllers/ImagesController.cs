using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Common;
using Website.Models.Database;
using Website.Services;

namespace Website.Controllers;

[Authorize]
[Route("admin/images")]
public class ImagesController : Controller
{
    private readonly DatabaseContext _context;
    private readonly R2 _r2;

    public ImagesController(DatabaseContext context, R2 r2)
    {
        _context = context;
        _r2 = r2;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Images.ToListAsync());
    }

    [HttpGet("upload")]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost("upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload([Bind("DateTaken,Description")] Image image)
    {
        if (ModelState.IsValid)
        {
            image.Id = Guid.NewGuid();
            image.DateUploaded = DateTime.UtcNow;

            _context.Add(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(image);
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var image = await _context.Images.FindAsync(id);
        if (image == null)
        {
            return NotFound();
        }
        return View(image);
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("DateTaken,Description")] Image image)
    {
        if (id != image.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(image);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(image.Id))
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
        return View(image);
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var image = await _context.Images
            .FirstOrDefaultAsync(m => m.Id == id);
        if (image == null)
        {
            return NotFound();
        }

        return View(image);
    }

    [HttpPost("delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var image = await _context.Images.FindAsync(id);
        if (image != null)
        {
            _context.Images.Remove(image);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ImageExists(Guid id)
    {
        return _context.Images.Any(e => e.Id == id);
    }
}
