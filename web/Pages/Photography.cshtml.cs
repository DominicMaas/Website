using Microsoft.AspNetCore.Mvc.RazorPages;
using Website.Models;

namespace Website.Pages;

public class PhotographyModel : PageModel
{
    public List<PhotoGroup> PhotosGroups { get; } = new List<PhotoGroup>()
    {
        new PhotoGroup("Nature", null, new()
        {
            new Photo("/images/gallery/nature/001.jpg", null),
            new Photo("/images/gallery/nature/002.jpg", null),
            new Photo("/images/gallery/nature/003.jpg", null),
            new Photo("/images/gallery/nature/004.jpg", null),
        }),
        new PhotoGroup("Astrophotography", "Photographs of celestial objects and the sky.", new()
        {
            new Photo("/images/gallery/astro/001.jpg", null),
            new Photo("/images/gallery/astro/002.jpg", null),
        }),
    };

    public void OnGet()
    {
    }
}
