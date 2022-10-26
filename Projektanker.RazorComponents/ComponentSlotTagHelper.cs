using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Projektanker.RazorComponents;

[HtmlTargetElement(Attributes = SlotAttributeName)]
public class ComponentSlotTagHelper : RazorComponentTagHelper
{
    public const string SlotAttributeName = "slot";

    public ComponentSlotTagHelper() : base(null)
    {
    }

    protected override void ProcessComponent(TagHelperContext context, TagHelperOutput output)
    {
        if (ParentComponent == null)
        {
            return;
        }

        var slotName = output
            .Attributes[SlotAttributeName]
            .Value
            .ToString();

        if (string.IsNullOrEmpty(slotName))
        {
            return;
        }

        ParentComponent.NamedSlots.Add(slotName, ChildContent);
        output.Attributes.RemoveAll(SlotAttributeName);
        output.SuppressOutput();
    }
}
