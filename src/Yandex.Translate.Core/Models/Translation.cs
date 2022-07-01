namespace Yandex.Translate.Core.Models;

public record Translation(
    string SourceString,
    string[] PossibleTranslations,
    string SourceLanguageCode,
    string TargetLanguageCode,
    TextFormat Format
)
{
    public string? First() => PossibleTranslations.FirstOrDefault();
};