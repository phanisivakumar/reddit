using Newtonsoft.Json;

namespace Analytics.Services.Reddit.Services;

public class SportsNews
{
    public static async IAsyncEnumerable<(string, string, int)> GetSportsData(string subreddit, string accessToken)
    {
        string baseUrl = "https://oauth.reddit.com";
        string subredditUrl = $"{baseUrl}/r/{subreddit}/.json";

        using (HttpClient _httpClient = new HttpClient())
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "YOUR_USER_AGENT");

            using (var response = await _httpClient.GetAsync(subredditUrl))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var sportsDataList = new List<(string, string, int)>();
                bool readingData = false;
                bool readingChildren = false;

                string title = null;
                string author = null;
                int numComments = 0;

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = jsonReader.Value.ToString();

                        if (propertyName == "title")
                        {
                            jsonReader.Read(); // Move to property value
                            title = jsonReader.Value.ToString();
                        }
                        else if (propertyName == "author")
                        {
                            jsonReader.Read(); // Move to property value
                            author = jsonReader.Value.ToString();
                        }
                        else if (propertyName == "num_comments")
                        {
                            jsonReader.Read(); // Move to property value
                            numComments = Convert.ToInt32(jsonReader.Value);

                            yield return (title, author, numComments);
                            // Add the data to the list and reset variables
                            //sportsDataList.Add((title, author, numComments));
                            title = null;
                            author = null;
                            numComments = 0;
                        }
                    }
                    else if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        if (readingChildren)
                        {
                            readingData = true;
                        }
                    }
                    else if (jsonReader.TokenType == JsonToken.EndObject)
                    {
                        if (readingData)
                        {
                            readingData = false;
                        }
                        else if (readingChildren)
                        {
                            readingChildren = false;
                        }
                    }
                    else if (jsonReader.TokenType == JsonToken.StartArray && jsonReader.Path == "data.children")
                    {
                        readingChildren = true;
                    }
                }
            }
        }
    }
}