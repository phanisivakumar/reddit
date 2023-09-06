namespace Analytics.Services.Reddit.Model;

public class ApiLimit
{
    public int RateLimitRemaining { get; set; }
    public int RateLimitReset { get; set; }
    public int RateLimitUsed { get; set; }
}