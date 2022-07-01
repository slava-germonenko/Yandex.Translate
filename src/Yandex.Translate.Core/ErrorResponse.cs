namespace Yandex.Translate.Core;

internal record ErrorResponse
{
    public string Message { get; set; } = string.Empty;
};