# HybridModelBinding

*For those who want the utmost flexibility in model binding.*

The hybrid approach differs from traditional model binding since you get to work with both `IModelBinder` and `IValueProvider`. This means your model can first get bound with data from the body of a request, and then get updated with data from route or querystring-attributes (in that order). This has the most benefit for users who prefer to have one-model for their request representations.

## Examples

### ASP.NET Core 3.1

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

        /**
         * This will also register a MVC-convention to auto-apply hybrid-binding behavior when a controller-action has a single parameter.
         * There are additional rules that get enforced to make sure the class is instantiable before applying this convention.
         */
        .AddHybridModelBinder(options =>
        {
            /**
             * This is optional and overrides internal ordering of how binding gets applied to a model that doesn't have explicit binding-rules.
             * Internal ordering is: body => form-values => route-values => querystring-values => header-values
             */
            options.FallbackBindingOrder = new[] { Source.QueryString, Source.Route };
        });
}
```

#### Model

```csharp
using HybridModelBinding;

[HybridBindClass(defaultBindingOrder: new[] { Source.Header, Source.Body, Source.Form, Source.QueryString, Source.Route })]
public class IndexModel
{
    // Binding will result from ordering specified at the class-level.
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

##### HybridBindClass

This attribute is optional. It allows specifying default-ordering when a property is not decorated with `HybridBindProperty`. This will override `FallbackBindingOrder`.

##### HybridBindProperty

This attribute is optional. It will override `HybridBindClass` and `FallbackBindingOrder`. It also allows specifying order. This is implicitly set based on the line-number of the attribute. It can also be explicitly set to avoid confusion:

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

##### IHybridBoundModel

Maybe you want to see how a property received its value. If your model implements `IHybridBoundModel`, this will give you what you seek. This is optional and not required for everything else to work.

```csharp
public class IndexModel : IHybridBoundModel
{
    public int? Age { get; set; }

    [HybridBindProperty(Source.Header, "X-Name")]
    [HybridBindProperty(new[] { Source.Body, Source.Form, Source.QueryString, Source.Route })]
    public string Name { get; set; }

    /**
     * Key is the property-name
     * Value is the source of the binding
     */
    public IDictionary<string, string> HybridBoundProperties { get; } = new Dictionary<string, string>();
}
```

#### Controller

```csharp
[HttpGet]
[Route("{age?}/{name?}")]
public IActionResult Index(IndexModel model)
{ }

/**
 * This action needs to declare `[FromHybrid]` since the registered convention won't hook it up to the library.
 * Without the parameter-attribute, default .NET behavior will get applied—even if your model is decorated with hybrid-attributes.
 */
[HttpGet]
[Route("{age?}/{name?}")]
public IActionResult IndexAlternate(int age, [FromHybrid]IndexModel model)
{ }
```

#### View

```html
<h3>Age: @(Model.Age.HasValue ? Model.Age.ToString() : "N/A")</h3>
<h3>Name: @(string.IsNullOrEmpty(Model.Name) ? "N/A" : Model.Name)</h3>
```

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