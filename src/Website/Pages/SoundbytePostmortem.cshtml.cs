using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages;

public class SoundbytePostmortem : PageModel
{
    public bool IsSoundByteContext { get; set; }

    public void OnGet(bool? isSoundByteContext)
    {
        IsSoundByteContext = isSoundByteContext ?? false;
    }
}
