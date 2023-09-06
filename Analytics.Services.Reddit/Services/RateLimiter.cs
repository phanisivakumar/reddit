using Analytics.Services.Reddit.Model;

namespace Analytics.Services.Reddit.Services;

public class RateLimiter
{
    public async Task<ApiLimit> CheckRateLimits(string accessToken)
    {
        string baseUrl = "https://oauth.reddit.com";
        string topPostsUrl = $"{baseUrl}/api/v1/me";
        
        var apiLimit = new ApiLimit();
        
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.DefaultRequestHeaders.Add("User-Agent", "YOUR_USER_AGENT");

            using (var response = await client.GetAsync(topPostsUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                apiLimit.RateLimitUsed = int.Parse(response.Headers.GetValues("x-ratelimit-used").FirstOrDefault());
                apiLimit.RateLimitRemaining = (int)double.Parse(response.Headers.GetValues("x-ratelimit-remaining").FirstOrDefault());
                apiLimit.RateLimitReset = int.Parse(response.Headers.GetValues("x-ratelimit-reset").FirstOrDefault());
            }
        }
        
        return apiLimit;
    }
}