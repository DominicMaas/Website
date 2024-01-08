using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Website.Common;
using Website.Services;
using Htmx;
using Image = Website.Models.Database.Image;

namespace Website.Controllers;

[Authorize]
[Route("admin/images")]
public class ImagesController : Controller
{
    private readonly DatabaseContext _context;
    private readonly R2 _r2;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(DatabaseContext context, R2 r2, ILogger<ImagesController> logger)
    {
        _context = context;
        _r2 = r2;
        _logger = logger;
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
    public async Task<IActionResult> Upload([Bind("DateTaken,Description,ImageFile")] ImageUpload imageUpload, CancellationToken cancellationToken)
    {
        // Run basic validation
        if (!ModelState.IsValid) return View(imageUpload);

        // Ensure we have an image file
        if (imageUpload.ImageFile == null)
        {
            ModelState.AddModelError(nameof(imageUpload.ImageFile), "An image file is required");
            return View(imageUpload);
        }

        // TODO: Ensure image was actually uploaded

        using var imageStream = new MemoryStream();

        try
        {
            // Start processing the uploaded image. We want to strip meta data, and convert to a jpeg. we also
            // want to create a thumbnail as well.
            using var image = SixLabors.ImageSharp.Image.Load(imageUpload.ImageFile.OpenReadStream());

            // Resize to an appropriate max size of 1000px
            image.Mutate(x => x.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(1000) }));

            image.Metadata.ExifProfile = null;
            image.Metadata.XmpProfile = null;

            // Save the image to a memory stream
            await image.SaveAsJpegAsync(imageStream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 80 }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing an image");

            ModelState.AddModelError(nameof(imageUpload.ImageFile), "An error occurred while processing this image. Please try again later.");
            return View(imageUpload);
        }

        // Handle cancel during processing
        if (cancellationToken.IsCancellationRequested)
        {
            ModelState.AddModelError(string.Empty, "The upload was canceled!");
            return View(imageUpload);
        }

        imageUpload.Id = Guid.NewGuid();
        imageUpload.DateUploaded = DateTime.UtcNow;

        try
        {
            // Attempt to upload the image
            await _r2.UploadImageAsync(imageStream, $"i/{imageUpload.Id}.jpg", "image/jpeg", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading an image to R2");

            ModelState.AddModelError(nameof(imageUpload.ImageFile), "An error occurred while uploading this image to R2. Please try again later.");
            return View(imageUpload);
        }

        // Handle cancel during R2 upload
        if (cancellationToken.IsCancellationRequested)
        {
            ModelState.AddModelError(string.Empty, "The upload was canceled!");
            return View(imageUpload);
        }

        _context.Add(imageUpload);
        await _context.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index));
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

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? id, CancellationToken cancellationToken)
    {
        if (id == null)
        {
            return NotFound();
        }

        var image = await _context.Images.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (image == null)
        {
            return NotFound();
        }

        // TODO: Check if used in streams or gallery

        try
        {
            await _r2.DeleteImageAsync($"i/{image.Id}.jpg", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting an image from R2");
            return NotFound();
        }

        _context.Images.Remove(image);
        await _context.SaveChangesAsync(cancellationToken);

        Response.Htmx(headers =>
        {
            headers.Refresh();
        });

        return Ok();
    }

    private bool ImageExists(Guid id)
    {
        return _context.Images.Any(e => e.Id == id);
    }

    public class ImageUpload : Image
    {
        [NotMapped]
        [DisplayName("Image")]
        [Required(ErrorMessage = "An image file is required")]
        public IFormFile? ImageFile { get; set; }
    }
}
