using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages;

public class ExperienceModel : PageModel
{
    public void OnGet(bool show = false)
    {
        ViewData["IsLookingForWork"] = false;

        // Just override this variable
        if (show == true)
            ViewData["IsLookingForWork"] = true;
    }
}
