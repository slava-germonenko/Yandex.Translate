namespace Yandex.Translate.Core;

internal record SuccessLangDetectionResponse
{
    public string Lang { get; set; } = string.Empty;
}