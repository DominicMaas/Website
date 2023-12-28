using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Modals;

public class ImageModalModel : PageModel
{
    public string? Source { get; set; }

    public string? Alt { get; set; }

	public void OnGet(string? src, string? alt)
    {
        Source = src;
        Alt = alt;
	}
}
