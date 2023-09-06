using Analytics.Services.Reddit.Model;
using Microsoft.Extensions.Options;

namespace Analytics.Services.Reddit.Services;

public class Tokenizer
{
    //private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    //private readonly string _redirectUri;
    
    private readonly string _tokenUrl;
    private readonly string _grantType;
    private readonly string _userAgent;
    private readonly string _authType;

    public Tokenizer(IOptions<RedditConfig> options)
    {
        //_logger = logger;
        _httpClient = new HttpClient();
        
        //// TODO: get this values from secret vault when it stored securely.
        _clientId = options.Value.ClientId;
        _clientSecret = options.Value.ClientSecret;

        _authType = options.Value.AuthType;
        _tokenUrl = options.Value.TokenUrl;
        _grantType = options.Value.GrantType;
        _userAgent = options.Value.UserAgent;
    }

    public async Task<string> GetAccessToken()
    {
        var authContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", _grantType)
        });

        _httpClient.DefaultRequestHeaders.Add("User-Agent", _userAgent);

        var authValue = new System.Net.Http.Headers.AuthenticationHeaderValue(_authType,
            Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}")));
        _httpClient.DefaultRequestHeaders.Authorization = authValue;

        var response = await _httpClient.PostAsync(_tokenUrl, authContent);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic? responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
            
            if (responseData == null) return string.Empty;
            
            string accessToken = responseData.access_token;
            return accessToken;
        }
        else
        {
            throw new Exception($"Token request failed with status code: {response.StatusCode}");
        }
    }
}