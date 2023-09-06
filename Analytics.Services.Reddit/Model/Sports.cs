using Newtonsoft.Json;

namespace Analytics.Services.Reddit.Model;

public class RedditApiResponse
{
    public RedditData Data { get; set; }
}

public class RedditData
{
    public RedditPost[] Children { get; set; }
}

public class RedditPost
{
    public string Kind { get; set; }
    public RedditPostData Data { get; set; }
}

public class RedditPostData
{
    public string Title { get; set; }
    public string Author { get; set; }

    [JsonProperty("num_comments")]
    public int NumComments { get; set; }
}