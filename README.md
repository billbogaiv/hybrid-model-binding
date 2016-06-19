# HybridModelBinding

*For those who want the utmost flexibility in model binding.*

**Note: this README is a [WIP](http://stackoverflow.com/a/15763080).**

The hybrid approach differs from traditional model binding since you get to work with both `IModelBinder` and `IValueProvider`. This means your model can first get bound with data from the body of a request, and then potentially get updated with data from route or querystring-attributes (in that order). This has the most benefit for users who prefer to have one-model for their request representations. **To avoid unexpected model-values, value providers by default, are not bound unless the model's properties are properly attribute-decorated.**

## Examples

### ASP.NET Core 1.0

#### project.json

```json
{
    "dependencies": {
        "HybridModelBinding": "0.1.0-*"
    }
}
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
        var readerFactory = services.BuildServiceProvider().GetRequiredService<IHttpRequestStreamReaderFactory>();

        x.ModelBinderProviders.Insert(0, new DefaultHybridModelBinderProvider(readerFactory));
    });
}
```

#### Model

```csharp
using Microsoft.AspNetCore.Mvc;

public class Person
{
    [FromQuery]
    public int Age { get; set; }

    [FromRoute]
    public string Name { get; set; }
}
```

#### Controller

```csharp
[HttpPost]
[Route("people/{name}")]
public IActionResult Post(Person model)
{ }
```

By adding the convention-instance, any controller-action with one-parameter will automatically get associated with the hybrid binder. `DefaultHybridModelBinderProvider` is built-in and is setup to bind data in the following order: body => form-values => route-values => querystring-values. If you do not want to use the convention, you can also decorate your model-parameter with `[FromHybrid]`.