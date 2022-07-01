# Yandex.Translate.Extensions.DependencyInjection

Dependency Injection extensions for for the Yandex.Translate package.

This package adds `AddTranslateClient` method overloads 
to facilitate work with the .NET DI mechanism.

There are 2 overloads of this method:

The first one and the easies to used simple accepts API key as param
```csharp
services.AddTranslateClient("YOUR_API_KEY");
``` 
In addition to that, you can pass second parameter which specifies 
what `ServiceLifetime` should translate client have. `ServiceLifetime.Scoped` is the default.

The second one accepts `TranslateClientConfiguration` as the parameter.
```csharp
var config = new TranslateClientConfiguration("YOUR_API_KEY");
services.AddTranslateClient(config);
``` 

In addition to that, you can pass second parameter which specifies
what `ServiceLifetime` should translate client have. `ServiceLifetime.Scoped` is the default.
