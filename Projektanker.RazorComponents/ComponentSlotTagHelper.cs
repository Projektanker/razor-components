using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Projektanker.RazorComponents;

[HtmlTargetElement("slot", Attributes = ModelAttributeName)]
public class ComponentSlotTagHelper : TagHelper
{
    public const string ModelAttributeName = "model";

    [HtmlAttributeName(ModelAttributeName)]
    public RazorComponentTagHelper? Model { get; set; }

    public string? Name { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (Model == null)
        {
            return;
        }

        IHtmlContent? content = null;
        if (string.IsNullOrEmpty(Name))
        {
            content = Model.ChildContent;
        }
        else if (Model.NamedSlots.ContainsKey(Name))
        {
            content = Model.NamedSlots[Name];
        }

        content ??= await output.GetChildContentAsync();

        output.Content.SetHtmlContent(content);
        output.TagName = null;
    }
}
