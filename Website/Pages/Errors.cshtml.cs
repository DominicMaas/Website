using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages
{
    public class Errors : PageModel
    {
        public string Code { get; set; }
        
        public void OnGet(string code)
        {
            Code = code;
        }
    }
}