using System.Net.Http.Json;
using System.Text.Json.Nodes;

using Yandex.Translate.Core.Exceptions;
using Yandex.Translate.Core.Models;

namespace Yandex.Translate.Core;

/// <summary>
/// Yandex.Translate facade.
/// </summary>
public class TranslateClient
{
    private readonly Uri _baseTranslateUri = new("https://translate.yandex.net/api/v1.5/tr.json/translate");

    private readonly Uri _baseSupportedLanguagesUri = new("https://translate.yandex.net/api/v1.5/tr.json/getLangs");

    private readonly Uri _baseDetectLanguageUri = new("https://translate.yandex.net/api/v1.5/tr.json/detect");
    
    private readonly HttpClient _httpClient;

    private readonly TranslateClientConfiguration _configuration;

    private string ApiKey => _configuration.ApiKey;

    public TranslateClient(HttpClient httpClient, TranslateClientConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public TranslateClient(TranslateClientConfiguration configuration) : this(new HttpClient(), configuration) { }

    /// <summary>
    /// Get translations 
    /// </summary>
    /// <param name="text">Text to be translated.</param>
    /// <param name="sourceLangCode">Source language code, e.g. "en", "es", "ru".</param>
    /// <param name="targetLangCode">Target language code, e.g. "en", "es", "ru".</param>
    /// <param name="format">
    ///     Specifies whether text to be translated is a plain text or HTML.
    ///     <see cref="TextFormat"/>
    /// </param>
    /// <returns>
    ///     Translation result – <see cref="Models.Translation"/>
    /// </returns>
    /// <exception cref="TranslateException">
    ///     Thrown if an error occurs trying to communicate and retrieve response from the Yandex.Translate.
    /// </exception>
    /// <exception cref="TranslateApiException">
    ///     Thrown if Yandex.Translate return error result. It contains error message thrown by Yandex.Translate.
    /// </exception>
    public async Task<Translation> TranslateAsync(
        string text,
        string sourceLangCode,
        string targetLangCode,
        TextFormat format = TextFormat.Text
    )
    {
        var apiRequest = BuildTranslateApiRequest(text, sourceLangCode, targetLangCode, format);
        var response = await _httpClient.SendAsync(apiRequest);

        if (!response.IsSuccessStatusCode)
        {
            throw await GetExceptionToThrow(response.Content);
        }

        var translateResult = await response.Content.ReadFromJsonAsync<SuccessTranslateResponse>();
        if (translateResult is null)
        {
            throw new TranslateException(ErrorMessages.FailedToSerializeResponse);
        }

        return new(text, translateResult.Text, sourceLangCode, targetLangCode, format);
    }

    /// <summary>
    /// Retrieves supported languages from the Yandex.Translate API.
    /// </summary>
    /// <param name="displayLanguageCode">
    ///     Language that will be used by Yandex.Translate to generate languages metadata in.
    /// </param>
    /// <returns>
    ///     <see cref="Models.SupportedLanguagesData"/> object.
    /// </returns>
    /// <exception cref="TranslateException">
    ///     Thrown if an error occurs trying to communicate and retrieve response from the Yandex.Translate.
    /// </exception>
    public async Task<SupportedLanguagesData> GetSupportedLanguages(string displayLanguageCode = "en")
    {
        var apiRequest = BuildSupportedLanguagesApiRequest(displayLanguageCode);
        var response = await _httpClient.SendAsync(apiRequest);
        if (!response.IsSuccessStatusCode)
        {
            throw await GetExceptionToThrow(response.Content);
        }

        var stringRepresentation = await response.Content.ReadAsStringAsync();
        if (stringRepresentation is null)
        {
            throw new TranslateException("Failed to read Yandex.Translate API response.");
        }

        var jsonContent = JsonNode.Parse(stringRepresentation);
        if (jsonContent is null or not JsonObject)
        {
            throw new TranslateException(ErrorMessages.FailedToSerializeResponse);
        }

        if (!TrySerializeSupportedLanguagesData(jsonContent.AsObject(), out var languagesData))
        {
            throw new TranslateException(ErrorMessages.FailedToSerializeResponse);
        }

        return languagesData;
    }

    /// <summary>
    /// Detects language of a given string.
    /// </summary>
    /// <param name="text">Text to be used as a reference.</param>
    /// <param name="hintLanguages">
    ///     Languages to be used as a hint.
    ///     When detecting a language, hint languages will be preferred.
    /// </param>
    /// <returns>Detected language code.</returns>
    /// <exception cref="TranslateException">
    ///     Thrown if an error occurs trying to communicate and retrieve response from the Yandex.Translate.
    /// </exception>
    public async Task<string> DetectLanguageAsync(string text, IEnumerable<string>? hintLanguages = null)
    {
        var apiRequest = BuildDetectLanguageApiRequest(text, hintLanguages);
        var response = await _httpClient.SendAsync(apiRequest);

        if (!response.IsSuccessStatusCode)
        {
            throw await GetExceptionToThrow(response.Content);
        }

        var detectResult = await response.Content.ReadFromJsonAsync<SuccessLangDetectionResponse>();
        if (detectResult is null)
        {
            throw new TranslateException(ErrorMessages.FailedToSerializeResponse);
        }

        return detectResult.Lang;
    }

    private HttpRequestMessage BuildTranslateApiRequest(
        string text,
        string sourceLangCode,
        string targetLangCode,
        TextFormat format = TextFormat.Text
    )
    {
        var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
        query.Add("key", ApiKey);
        query.Add("lang", $"{sourceLangCode}-{targetLangCode}");
        query.Add("format", format.ToString().ToLower());

        var requestUri = new UriBuilder(_baseTranslateUri) { Query = query.ToString() }.Uri;
        var textParameter = new KeyValuePair<string, string>("text", text);
        return new (HttpMethod.Post, requestUri)
        {
            Content = new FormUrlEncodedContent(new []{textParameter})
        };
    }

    private HttpRequestMessage BuildSupportedLanguagesApiRequest(string displayLanguageCode)
    {
        var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
        query.Add("key", ApiKey);
        query.Add("ui", displayLanguageCode);

        var requestUri = new UriBuilder(_baseSupportedLanguagesUri) { Query = query.ToString() }.Uri;
        return new(HttpMethod.Post, requestUri);
    }

    private HttpRequestMessage BuildDetectLanguageApiRequest(string text, IEnumerable<string>? hintLanguages)
    {
        var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
        query.Add("key", ApiKey);
        if (hintLanguages is not null && hintLanguages.Any())
        {
            query.Add("hint", string.Join(',', hintLanguages));   
        }
        
        var requestUri = new UriBuilder(_baseDetectLanguageUri) { Query = query.ToString() }.Uri;
        var textParameter = new KeyValuePair<string, string>("text", text);
        return new(HttpMethod.Post, requestUri)
        {
            Content = new FormUrlEncodedContent(new []{textParameter})
        };
    }
    
    private bool TrySerializeSupportedLanguagesData(
        JsonObject supportedLanguagesJson,
        out SupportedLanguagesData supportedLanguagesData
    )
    {
        supportedLanguagesData = new SupportedLanguagesData(
            new List<Language>(),
            new List<TranslateDirection>()
        );

        var directionsNode = supportedLanguagesJson["dirs"];
        var languagesNode = supportedLanguagesJson["langs"];
        if (directionsNode is not JsonArray directions || languagesNode is not JsonObject languages)
        {
            return false;
        }

        foreach (var direction in directions.Where(d => d != null).Select(d => d!.GetValue<string>()))
        {
            // Should be in format langCode-langCode, e.g. "en-ru"
            var directionLanguages = direction.Split('-');
            var translateDirection = new TranslateDirection(directionLanguages[0], directionLanguages[1]);
            supportedLanguagesData.Directions.Add(translateDirection);
        }

        var languageCodesAndNames = languages.Where(node => node.Value != null).Select(node => (node.Key, node.Value!.GetValue<string>()));
        foreach (var (code, name) in languageCodesAndNames)
        {
            supportedLanguagesData.Languages.Add(new Language(code, name));
        }

        return true;
    }

    private async Task<TranslateException> GetExceptionToThrow(HttpContent responseContent)
    {
        var error = await responseContent.ReadFromJsonAsync<ErrorResponse>();
        return error is null 
            ? new TranslateException(ErrorMessages.FailedToSerializeResponse)
            : new TranslateApiException(error.Message);
    }
}