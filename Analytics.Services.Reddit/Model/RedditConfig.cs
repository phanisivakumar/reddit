namespace Analytics.Services.Reddit.Model;

public record RedditConfig
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string AuthType { get; init; } = string.Empty;
    public string TokenUrl { get; init; } = string.Empty;
    public string GrantType { get; init; } = string.Empty;
    public string UserAgent { get; init; } = string.Empty;
}