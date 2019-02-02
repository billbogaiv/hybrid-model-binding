# HybridModelBinding

*For those who want the utmost flexibility in model binding.*

The hybrid approach differs from traditional model binding since you get to work with both `IModelBinder` and `IValueProvider`. This means your model can first get bound with data from the body of a request, and then potentially get updated with data from route or querystring-attributes (in that order). This has the most benefit for users who prefer to have one-model for their request representations.

## Examples

### ASP.NET Core 2.2

```shell
dotnet add package HybridModelBinding
```

#### Startup.cs

```csharp

// Boilerplate...

public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services
        .AddMvc()
        .AddHybridModelBinder();
}
```

#### Model

##### NEW WAY!

(version 0.14.0+) New implementation will allow a bit more flexibility and expandability for future features.

```csharp
using HybridModelBinding;

public class IndexModel
{
    [HybridBindProperty(new[] { Source.Header, Source.Body, Source.Form, Source.QueryString, Source.Route })]
    public int? Age { get; set; }

    /// <summary>
    /// For this specific source, we want to bind to a header-key of `X-Name`.
    /// Additionally, this has higher-precedence when looking for a binding-source.
    /// </summary>
    [HybridBindProperty(Source.Header, "X-Name")]

    /// <summary>
    /// These are bound using the source-key of `Name`.
    /// </summary>
    [HybridBindProperty(new[] { Source.Body, Source.Form, Source.QueryString, Source.Route })]
    public string Name { get; set; }
}
```

`HybridBindProperty` also allows specifying order. This is implicitly set based on the line-number of the attribute. It can also be explicitly set to avoid confusion:

```csharp
[HybridBindProperty(Source.Header, "X-Name", order: 5)]
[HybridBindProperty(new[] { Source.Body, Source.Form, Source.QueryString, Source.Route }, order: 10)]
public string Name { get; set; }
```

Declaring multiple attributes on the same line may cause unpredictable results and should be avoided (unless using explicit ordering):

```csharp
[HybridBindProperty(Source.Header, "X-Name")][HybridBindProperty(new[] { Source.Body, Source.Form, Source.QueryString, Source.Route })]
public string Name { get; set; }
```

This way is also valid, but may look odd depending whether you read lists top-bottom or bottom-top. In this case, the `Source.Body...`-attribute is given higher priority.

```csharp
[HybridBindProperty(Source.Header, "X-Name", order: 10)]
[HybridBindProperty(new[] { Source.Body, Source.Form, Source.QueryString, Source.Route }, order: 5)]
public string Name { get; set; }
```

##### ⚠️ Obsolete way

No specific timeline for complete removal, but `[From]` will **not** be getting updates.
`[From]` and `[HybridBindProperty]` *can* be used together on the same property. Ordering rules will apply `[From]` first.

```csharp
using HybridModelBinding;

public class IndexModel
{
    [From(Source.Body, Source.QueryString, Source.Route)]
    public int? Age { get; set; }

    // Specific re-ordering to show `route` comes first!
    [From(Source.Body, Source.Form, Source.Route, Source.QueryString)]
    public string Name { get; set; }
}
```

#### Controller

```csharp
[HttpGet]
[Route("{age?}/{name?}")]
public IActionResult Index(IndexModel model)
{ }
```

#### View

```html
<h3>Age: @(Model.Age.HasValue ? Model.Age.ToString() : "N/A")</h3>
<h3>Name: @(string.IsNullOrEmpty(Model.Name) ? "N/A" : Model.Name)</h3>
```

By adding the convention-instance, any controller-action with one-parameter will automatically get associated with the hybrid binder. `DefaultHybridModelBinderProvider` is built-in and is setup to bind data in the following order: body => form-values => route-values => querystring-values => header-values. If you do not want to use the convention, you can also decorate your model-parameter with `[From]`.

## Results

Based on the setup above, here is how various URIs will get parsed/rendered:

### /

```html
<h3>Age: N/A</h3>
<h3>Name: N/A</h3>
```

### /10

```html
<h3>Age: 10</h3>
<h3>Name: N/A</h3>
```

### /10/Bill

```html
<h3>Age: 10</h3>
<h3>Name: Bill</h3>
```

### /10/Bill?age=20

```html
<h3>Age: 20</h3>
<h3>Name: Bill</h3>
```

### /10/Bill?age=20&name=Boga

```html
<h3>Age: 20</h3>
<h3>Name: Bill</h3> <!--note how the querystring does not get bound since the route comes first in the [From...] ordering-->
```

### Example using headers and alternate-naming

```curl
curl -X GET \
  http://localhost/10/Bill?name=Boga \
  -H 'X-Name: Billy'
  ```

  ```html
<h3>Age: 20</h3>
<h3>Name: Billy</h3>
```