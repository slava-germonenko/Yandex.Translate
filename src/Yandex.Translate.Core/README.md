# Yandex.Translate Client

This is a simple client for the Yandex.Translate service.

## Core Features

All core features share the same client class `TranslateClient`.
To start using it simply create a new instance of the class:
1. At first, create configuration object (for now it contains API key property only)
2. Then simply create `TranslateClient` instance.
```csharp
var config = new TranslateClientConfiguration("YOUR_API_KEY");
var client = new TranslateClient(config);
```

### Translation

To translate text you can use `TranslateClient.TranslateAsync(text, sourceLang, targetLang, format = text)` method
which returns `Translation` object that contains possible translations and some additional metadata 
like source string, source and target language codes and format.
```csharp
var translation = await client.TranslateAsync("Hellow World!", "en", "ru");
Console.WriteLine(translation.First()!) // Output: "Привет, мир!"
```

`TranslateClient.TranslateAsync` Accepts the following arguments:
1. Text to be translated (`string`);
2. Source language code (`string`);
3. Target language code (`string`);
4. Translate format, which specifies whether text to be translated is a plain text or HTML (`TextFormat` enum).

### Language Detection

To detect language you can use `TranslateClient.DetectLanguageAsync(text, hintLanguages)` method
which returns detected language code.
```csharp
var detectedLanguage = await client.DetectLanguageAsync("Hello World!");
Console.WriteLine(detectedLanguage) // Output: "en"
```

`DetectLanguageAsync` accepts the following parameters:
1. Text which is used as reference for language detection (`string`);
2. Hint languages. When detecting language, hint language will be preferred (`IEnumerable<string>`).

### Supported Languages

To get supported languages you can use the `TranslateClient.GetSupportedLanguages(displayLanguageCode = "en")` method:
```csharp
var supportedLanguagesData = await client.GetSupportedLanguages();
foreach(var language in supportedLanguagesData.Languages) {
    Console.WriteLine(language.Code) // Output: "en", "ru", "pt"
    Console.WriteLine(language.Name) // Output: "English", "Russian", "Protugeese".
}
foreach(var direction in supportedLanguagesData.Directions) {
    Console.WriteLine(direction.SourceLanguage) // Output: "en", "en", "en"
    Console.WriteLine(direction.TargetLanguage) // Output: "ru", "pt", "es"
}
```
`GetSupportedLanguages` accepts `displayLanguageCode` 
which specifies which language will be used to display language names.

The method returns `SupportedLanguagesData` object which contains supported languages and supported directions.
