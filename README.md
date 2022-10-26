# Razor Components

![NuGet](https://badgen.net/nuget/v/Projektanker.RazorComponents)

Razor Components is an ASP.NET Core library that allows you to write UI components while maintaining compatibility with Razor Pages and MVC.

## Installation

Install the [NuGet package](https://www.nuget.org/packages/Projektanker.RazorComponents)  
```bash
dotnet add package Projektanker.RazorComponents
```

Razor Components works by using tag helpers.  
As with all tag helpers, you will need to go to the `_ViewImports.cshtml` file and add a reference to the package's and your project's namespace like so:

```csharp
@addTagHelper *, Projektanker.RazorComponents
@addTagHelper *, Sample.Web
```

## Usage

### Basic usage without child content

Given folder structure:  
```
~/Views/Components
  - HelloWorld.cshtml
  - HelloWorld.cshtml.cs
```

`HelloWorld.cshtml.cs`:  
```csharp
namespace Sample.Web.Views.Components;

[HtmlTargetElement("HelloWorld")]
public class HelloWorld : RazorComponentTagHelper
{
    public string Greeting { get; set; } = string.Empty;
}
```

`HelloWorld.cshtml`:  
```html
@using Sample.Web.Views.Components
@model HelloWorld
<p>
  <strong>@Model.Greeting</strong> World
</p>
```

You would use it like this:
```html
<HelloWorld greeting="Hello" />
```

### Basic usage with single child content
Given folder structure:  
```
~/Views/Components
  - Button.cshtml
  - Button.cshtml.cs
```

`Button.cshtml.cs`:  
```csharp
namespace Sample.Web.Views.Components;

[HtmlTargetElement("Button")]
public class Button : RazorComponentTagHelper
{
}
```

`Button.cshtml`:  
```html
@using Sample.Web.Views.Components
@model Button
<button class="btn btn-primary">
  @Model.ChildContent
</button>
```

You would use it like this:
```html
<Button>
  Click me
</Button>
```

### Advanced usage with named slots

Given folder structure:  
```
~/Views/Components
  - Card.cshtml
  - Card.cshtml.cs
```

`Card.cshtml.cs`:  
```csharp
namespace Sample.Web.Views.Components;

[HtmlTargetElement("Card")]
public class Card : RazorComponentTagHelper
{
}
```

`Card.cshtml`:  
```html
@using Sample.Web.Views.Components
@model Card
<div class="card">
  <h3>@Model.NamedSlots["title"]</h3>
  @Model.NamedSlots["content"]
</div>
```

You would use it like this:
```html
<Card>
  <fragment slot="title">
    Card title
  </fragment>
  <div slot="content">
    <p>Card content</p>
    <p>Lorem ipsum</p>
  </div>
</Card>
```

The `<fragment>` element allows you to place content in a named slot without wrapping it in a container DOM element. This keeps the flow layout of your document intact.

## Inspiration

- [Acmion/CshtmlComponent](https://github.com/Acmion/CshtmlComponent/)
- [techgems/razor-component-tag-helpers](https://github.com/techgems/razor-component-tag-helpers/)
- [sveltejs/svelte](https://github.com/sveltejs/svelte)
