using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website.Common;
using Website.Models;

namespace Website.Pages
{
    public class BlogList : PageModel
    {
        public IEnumerable<BlogPostFrontMatter> BlogPostsMeta { get; private set; }
        
        public void OnGet()
        {
            var files = Directory.GetFiles("wwwroot/blog");

            BlogPostsMeta = files
                .Select(System.IO.File.ReadAllText)
                .Select(md => md.GetFrontMatter<BlogPostFrontMatter>())
                .Where(x => x.Public);
        }
    }
}