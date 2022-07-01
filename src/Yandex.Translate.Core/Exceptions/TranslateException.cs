namespace Yandex.Translate.Core.Exceptions;

/// <summary>
/// Base translate client exception.
/// </summary>
public class TranslateException : Exception
{
    public TranslateException(string message) : base(message) { }
}