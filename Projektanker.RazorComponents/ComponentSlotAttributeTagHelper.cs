using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Projektanker.RazorComponents;

[HtmlTargetElement(Attributes = SlotAttributeName)]
public class ComponentSlotAttributeTagHelper : RazorComponentTagHelper
{
    public const string SlotAttributeName = "slot";

    public ComponentSlotAttributeTagHelper() : base(null)
    {
    }

    [HtmlAttributeName(SlotAttributeName)]
    public string? Slot { get; set; }

    protected override void ProcessComponent(TagHelperContext context, TagHelperOutput output)
    {
        if (ParentComponent == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(Slot))
        {
            return;
        }

        ParentComponent.NamedSlots.Add(Slot, ChildContent);
        output.Attributes.RemoveAll(SlotAttributeName);
        output.SuppressOutput();
    }
}
