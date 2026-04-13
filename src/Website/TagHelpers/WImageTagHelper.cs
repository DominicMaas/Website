using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Website.TagHelpers;

public class WImageTagHelper : TagHelper
{
    [HtmlAttributeName("webp-src")]
    public string? WebpSrc { get; set; }

    public string? Width { get; set; }

    public string? Height { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // Wrap everything in a <picture> tag
        output.TagName = "picture";

        // Force the picture tag to render with an opening and closing tag
        output.TagMode = TagMode.StartTagAndEndTag;

        output.Attributes.TryGetAttribute("src", out var srcAttr);
        output.Attributes.TryGetAttribute("alt", out var altAttr);

        var src = srcAttr?.Value?.ToString() ?? string.Empty;
        var alt = altAttr?.Value?.ToString() ?? string.Empty;

        // Build the actual img tag
        var imgTag = new TagBuilder("img");

        // Copy all original attributes from the tag helper to the inner img tag
        foreach (var attribute in output.Attributes)
        {
            imgTag.Attributes.Add(attribute.Name, attribute.Value.ToString() ?? string.Empty);
        }

        // Clear them from the <picture> Output so they don't apply to the wrapper
        output.Attributes.Clear();

        // Apply specific mapped attributes to the <img> tag
        if (!string.IsNullOrWhiteSpace(Width))
            imgTag.Attributes["width"] = Width;

        if (!string.IsNullOrWhiteSpace(Height))
            imgTag.Attributes["height"] = Height;

        // Apply our required classes and htmx interactivity to the img element
        imgTag.AddCssClass("image pure-img");
        imgTag.Attributes["hx-get"] = "/modals/image-modal";
        imgTag.Attributes["hx-target"] = "body";
        imgTag.Attributes["hx-swap"] = "beforeend";
        imgTag.Attributes["hx-vals"] = "{\"src\": \"" + src + "\", \"alt\": \"" + alt + "\"}";
        imgTag.TagRenderMode = TagRenderMode.SelfClosing;

        // Optionally inject the WebP source conditionally
        if (!string.IsNullOrWhiteSpace(WebpSrc))
        {
            var sourceTag = new TagBuilder("source");
            sourceTag.Attributes.Add("srcset", WebpSrc);
            sourceTag.Attributes.Add("type", "image/webp");

            // Replicate width and height to the <source> tag for optimal CLS handling
            if (!string.IsNullOrWhiteSpace(Width))
                sourceTag.Attributes["width"] = Width;

            if (!string.IsNullOrWhiteSpace(Height))
                sourceTag.Attributes["height"] = Height;

            sourceTag.TagRenderMode = TagRenderMode.SelfClosing;

            output.Content.AppendHtml(sourceTag);
        }

        // Output the img tag inside the picture tag
        output.Content.AppendHtml(imgTag);
    }
}
