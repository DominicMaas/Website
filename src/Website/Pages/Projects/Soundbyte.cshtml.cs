using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Projects;

public class Soundbyte : PageModel
{
    public bool IsSoundByteContext { get; set; }

    public void OnGet(bool? isSoundByteContext)
    {
        IsSoundByteContext = isSoundByteContext ?? false;
    }
}
