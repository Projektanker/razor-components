using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Projektanker.RazorComponents;

[HtmlTargetElement("fragment")]
public class FragmentTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
    }
}
