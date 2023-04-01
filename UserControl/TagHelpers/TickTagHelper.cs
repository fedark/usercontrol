using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UserControl.TagHelpers;

public class TickTagHelper : TagHelper
{
    public bool Value { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.AddClass("symbol", HtmlEncoder.Default);

        if (Value)
        {
            output.Content.SetContent("✓");
        }
    }
}
