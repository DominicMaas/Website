using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages;

public class FwModel : PageModel
{
    public string? RequestedUri { get; set; }

    public string? Message { get; set; }

    public bool IsSuccess { get; set; }

    private readonly Dictionary<string, string> _links = new()
    {
        { "4toUN4j53iK", "https://github.com/DominicMaas/SpaceChunks" },
        { "yENyTb", "https://twitter.com/SoundByteUWP" },
        { "Y5jGLtoFXs", "https://dominicmaas.co.nz/privacy" },
        { "GvC5iXmJSo", "https://dominicmaas.co.nz/projects/soundbyte" },

        { "github", "https://github.com/DominicMaas" },
        { "twitter", "https://twitter.com/dominicjmaas" }
    };

    public void OnGet(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            RequestedUri = string.Empty;
            Message = "This link does not exist";
            IsSuccess = false;
            return;
        }

        var url = _links.GetValueOrDefault(id);

        if (string.IsNullOrEmpty(url))
        {
            // This URI does not exist
            RequestedUri = string.Empty;
            Message = "This link does not exist";
            IsSuccess = false;
            return;
        }

        RequestedUri = url;
        IsSuccess = true;
    }
}
