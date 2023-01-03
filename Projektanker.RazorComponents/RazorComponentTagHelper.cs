using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Projektanker.RazorComponents;

public abstract class RazorComponentTagHelper : TagHelper
{
    private static readonly string ParentComponentsStackKey
        = $"{typeof(RazorComponentTagHelper).FullName}.{nameof(ParentComponentsStackKey)}";

    private static readonly ConcurrentDictionary<Type, string> _razorViewNamesByType = new();

    private readonly string? _razorViewName;

    protected RazorComponentTagHelper()
    {
        _razorViewName = _razorViewNamesByType.GetOrAdd(GetType(), type =>
        {
            var assemblyName = type.Assembly.GetName().Name;
            var relativeTypeName = type.FullName![assemblyName!.Length..];
            return $"~{relativeTypeName.Replace('.', '/')}.cshtml";
        });
    }

    protected RazorComponentTagHelper(string? razorViewName)
    {
        _razorViewName = razorViewName;
    }

    public string? RazorViewName => _razorViewName;

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    /// <summary>
    /// Child content for rendering in the razor template.
    /// </summary>
    [HtmlAttributeNotBound]
    public TagHelperContent ChildContent { get; private set; }
        = new DefaultTagHelperContent();

    [HtmlAttributeNotBound]
    public IDictionary<string, TagHelperContent> NamedSlots { get; }
        = new Dictionary<string, TagHelperContent>();

    [HtmlAttributeNotBound]
    internal RazorComponentTagHelper? ParentComponent { get; set; }

    public override sealed void Init(TagHelperContext context)
    {
        if (TryGetParentComponentStack(context, out var parentComponentStack))
        {
            ParentComponent = parentComponentStack.Peek();
            parentComponentStack.Push(this);
        }
        else
        {
            ParentComponent = null;
            parentComponentStack = new Stack<RazorComponentTagHelper>();
            parentComponentStack.Push(this);
            SetParentComponentStack(context, parentComponentStack);
        }

        base.Init(context);
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ChildContent = await output.GetChildContentAsync();
        ProcessComponent(context, output);
        PopParentComponentStack(context);

        if (_razorViewName != null)
        {
            await RenderPartialView(_razorViewName, output);
        }
    }

    protected virtual void ProcessComponent(TagHelperContext context, TagHelperOutput output)
    {
    }

    private static bool TryGetParentComponentStack(TagHelperContext context, [MaybeNullWhen(false)] out Stack<RazorComponentTagHelper> stack)
    {
        if (context.Items.TryGetValue(ParentComponentsStackKey, out var value))
        {
            stack = (Stack<RazorComponentTagHelper>)value;
            return true;
        }

        stack = null;
        return false;
    }

    private static void SetParentComponentStack(TagHelperContext context, Stack<RazorComponentTagHelper> stack)
    {
        context.Items[ParentComponentsStackKey] = stack;
    }

    private static void PopParentComponentStack(TagHelperContext context)
    {
        var stack = (Stack<RazorComponentTagHelper>)context.Items[ParentComponentsStackKey];
        stack.Pop();
    }

    private IHtmlHelper GetHtmlHelper()
    {
        var htmlHelper = ViewContext!.HttpContext.RequestServices.GetService<IHtmlHelper>();
        (htmlHelper as IViewContextAware)?.Contextualize(ViewContext);
        return htmlHelper;
    }

    private async Task RenderPartialView(string partialViewName, TagHelperOutput output)
    {
        var content = await GetHtmlHelper().PartialAsync(partialViewName, this);
        output.Content.SetHtmlContent(content);
        output.TagName = null;
    }
}
