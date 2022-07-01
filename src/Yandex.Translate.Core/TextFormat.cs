namespace Yandex.Translate.Core;

/// <summary>
/// Enum used by <see cref="TranslateClient"/>
/// to specify whether text being translated is a plain text or HTML.
/// </summary>
public enum TextFormat
{
    Text,
    Html,
}