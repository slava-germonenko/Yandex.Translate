namespace Yandex.Translate.Core.Exceptions;

public class TranslateApiException : TranslateException
{
    public TranslateApiException(string message) : base(message) { }
}