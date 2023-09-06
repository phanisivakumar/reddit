using Newtonsoft.Json;

namespace Analytics.Services.Reddit.Services;

public class TopContributors
{
    public static async Task<List<(string, int)>> GetTopContributorsInfo(string subreddit, string accessToken)
    {
        string baseUrl = "https://oauth.reddit.com";
        string contributorsUrl = $"{baseUrl}/r/{subreddit}/about/contributors/.json";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.DefaultRequestHeaders.Add("User-Agent", "YOUR_USER_AGENT");

            var resp = await client.GetAsync(contributorsUrl, HttpCompletionOption.ResponseHeadersRead);
            var result = await resp.Content.ReadAsStringAsync();
                
            using (var response = await client.GetAsync(contributorsUrl, HttpCompletionOption.ResponseHeadersRead))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var contributorsData = new List<(string, int)>();
                bool readingContributors = false;

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.StartArray && jsonReader.Path == "data.children")
                    {
                        readingContributors = true;
                    }
                    else if (jsonReader.TokenType == JsonToken.EndArray && jsonReader.Path == "data.children")
                    {
                        readingContributors = false;
                    }
                    else if (readingContributors && jsonReader.TokenType == JsonToken.StartObject)
                    {
                        string name = string.Empty;
                        int postCount = 0;

                        while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
                        {
                            if (jsonReader.TokenType == JsonToken.PropertyName)
                            {
                                string propertyName = jsonReader.Value.ToString();

                                if (propertyName == "name")
                                {
                                    jsonReader.Read();
                                    name = jsonReader.Value.ToString();
                                }
                                else if (propertyName == "total_posts")
                                {
                                    jsonReader.Read();
                                    postCount = Convert.ToInt32(jsonReader.Value);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(name))
                        {
                            contributorsData.Add((name, postCount));
                        }
                    }
                }

                return contributorsData;
            }
        }
    }
}