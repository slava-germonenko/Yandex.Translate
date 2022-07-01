namespace Yandex.Translate.Core;

internal record SuccessTranslateResponse
{
    public string[] Text { get; set; } = Array.Empty<string>();
}