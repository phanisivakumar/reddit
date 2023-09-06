using Newtonsoft.Json;

namespace Analytics.Services.Reddit.Services;

public class TopVotes
{
    public static async Task<(string?, int)> GetTopVotes(string subreddit, string accessToken)
    {
        string baseUrl = "https://oauth.reddit.com";
        string topPostsUrl = $"{baseUrl}/r/{subreddit}/top/.json?t=all&limit=1";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.DefaultRequestHeaders.Add("User-Agent", "YOUR_USER_AGENT");

            using (var response = await client.GetAsync(topPostsUrl, HttpCompletionOption.ResponseHeadersRead))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                bool readingPosts = false;
                string title = null;
                int upvotes = 0;
                bool foundData = false;

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.StartArray && jsonReader.Path == "data.children")
                    {
                        readingPosts = true;
                    }
                    else if (jsonReader.TokenType == JsonToken.EndArray && jsonReader.Path == "data.children")
                    {
                        break; // Exit the outer loop when the list of posts is finished
                    }
                    else if (readingPosts && jsonReader.TokenType == JsonToken.StartObject)
                    {
                        while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
                        {
                            if (jsonReader.TokenType == JsonToken.PropertyName)
                            {
                                string propertyName = jsonReader.Value.ToString();

                                if (propertyName == "title")
                                {
                                    jsonReader.Read(); // Move to property value
                                    title = jsonReader.Value.ToString();
                                }
                                else if (propertyName == "ups")
                                {
                                    jsonReader.Read(); // Move to property value
                                    upvotes = Convert.ToInt32(jsonReader.Value);

                                    // Set the flag indicating that data was found
                                    foundData = true;

                                    // Exit the inner loop immediately
                                    break;
                                }
                            }
                        }
                    }

                    // Check if data was found and exit the outer loop
                    if (foundData)
                    {
                        break;
                    }
                }

                return (title, upvotes);
                // Process the found data or handle the case where no data was found
                // if (foundData)
                // {
                //     Console.WriteLine($"Title: {title}, Upvotes: {upvotes}");
                //
                //     return (title, upvotes);
                // }
                // else
                // {
                //     Console.WriteLine("No data found.");
                // }
            }

        }
    }
}