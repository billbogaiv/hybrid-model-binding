# HybridModelBinding

*For those who want the utmost flexibility in model binding.*

**Note: this README is a [WIP](http://stackoverflow.com/a/15763080).**

The hybrid approach differs from traditional model binding since you get to work with both `IModelBinder` and `IValueProvider`. This means your model can first get bound with data from the body of a request, and then potentially get updated with data from route or querystring-attributes (in that order). This has the most benefit for users who prefer to have one-model for their request representations. **To avoid unexpected model-values, value providers by default, are not bound unless the model's properties are properly attribute-decorated.**

## Examples

### ASP.NET Core 2.0

```shell
dotnet add package HybridModelBinding
```

#### Startup.cs

```csharp
using HybridModelBinding;

// Boilerplate...

public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc(x =>
    {
        x.Conventions.Add(new HybridModelBinderApplicationModelConvention());
    });

    services.Configure<MvcOptions>(x =>
    {
        var serviceProvider = services.BuildServiceProvider();
        var readerFactory = serviceProvider.GetRequiredService<IHttpRequestStreamReaderFactory>();

        x.ModelBinderProviders.Insert(0, new DefaultHybridModelBinderProvider(x.InputFormatters, readerFactory));
    });
}
```

#### Model

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

By adding the convention-instance, any controller-action with one-parameter will automatically get associated with the hybrid binder. `DefaultHybridModelBinderProvider` is built-in and is setup to bind data in the following order: body => form-values => route-values => querystring-values. If you do not want to use the convention, you can also decorate your model-parameter with `[From]`.

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