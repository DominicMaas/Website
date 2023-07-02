using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;

namespace Website.Pages;

public class Error : PageModel
{
    public string? Code { get; set; }

    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void OnGet(int code)
    {
        Code = $"{code} - {ReasonPhrases.GetReasonPhrase(code)}";
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
