using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Website.TagHelpers;

public class WImageTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "img";

        output.Attributes.TryGetAttribute("alt", out var altAttr);

        output.Attributes.SetAttribute("class", "image pure-img");
        output.Attributes.SetAttribute("hx-get", "/modals/image-modal");
        output.Attributes.SetAttribute("hx-target", "body");
        output.Attributes.SetAttribute("hx-swap", "beforeend");
        output.Attributes.SetAttribute("hx-vals", "{\"src\": \"" + output.Attributes["src"].Value + "\", \"alt\": \"" + (altAttr?.Value ?? string.Empty) + "\"}");
    }
}
