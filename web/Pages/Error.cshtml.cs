using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages
{
    public class Error : PageModel
    {
        public string Code { get; set; }
        
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        public void OnGet(string code)
        {
            Code = code;
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}