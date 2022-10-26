using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace Projektanker.RazorComponents;

public abstract class RazorComponentTagHelper : TagHelper
{
    private static readonly string ParentComponentsStackKey
        = $"{typeof(RazorComponentTagHelper).FullName}.{nameof(ParentComponentsStackKey)}";

    private readonly string? _razorViewName;
    private ViewContext? _viewContext;

    protected RazorComponentTagHelper()
    {
        var type = GetType();
        var assemblyName = type.Assembly.GetName().Name;
        var relativeTypeName = type.FullName![assemblyName!.Length..];
        _razorViewName = $"~{relativeTypeName.Replace('.', '/')}.cshtml";
    }

    protected RazorComponentTagHelper(string? razorViewName)
    {
        _razorViewName = razorViewName;
    }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext
    {
        get => _viewContext;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _viewContext = value;
        }
    }

    /// <summary>
    /// Child content for rendering in the razor template.
    /// </summary>
    [HtmlAttributeNotBound]
    public IHtmlContent? ChildContent { get; private set; }

    [HtmlAttributeNotBound]
    public IDictionary<string, IHtmlContent?> NamedSlots { get; }
        = new Dictionary<string, IHtmlContent?>();

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
        var htmlHelper = _viewContext!.HttpContext.RequestServices.GetService<IHtmlHelper>();
        ((IViewContextAware)htmlHelper).Contextualize(_viewContext);
        return htmlHelper;
    }

    private async Task RenderPartialView(string partialViewName, TagHelperOutput output)
    {
        var content = await GetHtmlHelper().PartialAsync(partialViewName, this);
        output.Content.SetHtmlContent(content);
        output.TagName = null;
    }
}
