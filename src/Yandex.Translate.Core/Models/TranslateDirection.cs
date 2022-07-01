namespace Yandex.Translate.Core.Models;

/// <summary>
/// Represents possible "from"->"to" translate direction.
/// </summary>
/// <param name="SourceLanguage">Language to be translated from.</param>
/// <param name="TargetLanguage">Language to be translated to.</param>
public record TranslateDirection(string SourceLanguage, string TargetLanguage);