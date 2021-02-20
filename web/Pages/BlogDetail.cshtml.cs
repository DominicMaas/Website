using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website.Common;
using Website.Models;

namespace Website.Pages
{
    public class BlogDetail : PageModel
    {
        public string Text { get; set; }
        public BlogPostFrontMatter FrontMatter { get; set; }
        
        public IActionResult OnGet(string path)
        {
            var builtPath = $"wwwroot/blog/{path}.md";

            if (!System.IO.File.Exists(builtPath))
                return NotFound();

            var postFile = System.IO.File.ReadAllText($"wwwroot/blog/{path}.md");
            FrontMatter = postFile.GetFrontMatter<BlogPostFrontMatter>();
            
            var pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();

            Text = Markdown.ToHtml(postFile, pipeline);
            return Page();
        }
    }
}