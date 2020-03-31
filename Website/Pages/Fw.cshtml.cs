using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Website.Pages
{
    public class FwModel : PageModel
    {
        public string RequestedUri { get; set; }

        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        private readonly Dictionary<string, string> _links = new Dictionary<string, string>
        {
            { "4toUN4j53iK", "https://github.com/DominicMaas/SpaceChunks" },
            { "yENyTb", "https://twitter.com/SoundByteUWP" },
            { "Y5jGLtoFXs", "https://soundbytemedia.com/pages/privacy-policy" },
            { "GvC5iXmJSo", "https://soundbytemedia.com/pages/support" }
        };

        public void OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                RequestedUri = string.Empty;
                Message = "No ID was passed in";
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
}