using Htmx;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Website.Common;
using Website.Models.Database;

namespace Website.Controllers;

[Authorize]
[Route("admin/short-links")]
public class ShortLinksController(DatabaseContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await context.ShortLinks.Select(x => new ShortLink
        {
            Id = x.Id,
            RedirectLink = x.RedirectLink,
            HitsCount = x.Hits.Count()
        }).ToListAsync());
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: AdminShortLinks/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,RedirectLink")] ShortLink shortLink)
    {
        if (ModelState.IsValid)
        {
            context.Add(shortLink);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(shortLink);
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var shortLink = await context.ShortLinks.FindAsync(id);
        if (shortLink == null)
        {
            return NotFound();
        }
        return View(shortLink);
    }

    // POST: AdminShortLinks/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost("{id}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,RedirectLink")] ShortLink shortLink)
    {
        if (id != shortLink.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(shortLink);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShortLinkExists(shortLink.Id))
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
        return View(shortLink);
    }


    [HttpPost("{id}/delete"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var shortLink = await context.ShortLinks.FindAsync(id);
        if (shortLink == null)
        {
            return NotFound();
        }

        context.ShortLinks.Remove(shortLink);
        await context.SaveChangesAsync();

        Response.Htmx(headers =>
        {
            headers.Refresh();
        });

        return Ok();
    }

    private bool ShortLinkExists(string id)
    {
        return context.ShortLinks.Any(e => e.Id == id);
    }
}