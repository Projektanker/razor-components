using Microsoft.AspNetCore.Razor.TagHelpers;
using Projektanker.RazorComponents;

namespace Sample.Web.Views.Components;

[HtmlTargetElement("HelloWorld")]
public class HelloWorld : RazorComponentTagHelper
{
    public string Greeting { get; set; } = string.Empty;
}
