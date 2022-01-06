using Microsoft.AspNetCore.Mvc.RazorPages;
using Website.Models;

namespace Website.Pages;

public class PhotographyModel : PageModel
{
    public List<PhotoGroup> PhotosGroups { get; } = new List<PhotoGroup>() {
        new PhotoGroup("Placeholders", "These photos are placeholders from placeimg.com", new()
        {
            new Photo("http://placeimg.com/350/480/any", "http://placeimg.com/350/480/any"),
            new Photo("http://placeimg.com/640/480/any", "http://placeimg.com/640/480/any"),
            new Photo("http://placeimg.com/300/600/any", "http://placeimg.com/300/600/any"),
        }),
        new PhotoGroup("Astrophotography", "Photographs of celestial objects and the sky", new()
        {
            new Photo("/images/gallery/astro/001_thumb.jpg", "/images/gallery/astro/001.jpg"),
            new Photo("/images/gallery/astro/002.jpg", "/images/gallery/astro/002.jpg"),
        }),
        new PhotoGroup("Nature", null, new()
        {
            new Photo("/images/gallery/nature/001.jpg", "/images/gallery/nature/001.jpg"),
            new Photo("/images/gallery/nature/002.jpg", "/images/gallery/nature/002.jpg"),
            new Photo("/images/gallery/nature/003.jpg", "/images/gallery/nature/003.jpg"),
            new Photo("/images/gallery/nature/004.jpg", "/images/gallery/nature/004.jpg"),
        })
    };

    public void OnGet()
    {
    }
}
