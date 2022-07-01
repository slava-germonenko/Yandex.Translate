using Yandex.Translate.Core;

var config = new TranslateClientConfiguration("");
var client = new TranslateClient(config);

var translation = await client.TranslateAsync("Hello World!", "en", "ru");
Console.WriteLine(translation.First());

var detectedLanguage = await client.DetectLanguageAsync(translation.First()!);
Console.WriteLine(detectedLanguage);
