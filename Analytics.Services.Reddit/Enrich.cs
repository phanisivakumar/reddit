using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Analytics.Services.Reddit;

public class Enrich
{
    public static string ExtractIdFromJson(string jsonString)
    {
        try
        {
            // Parse the JSON string into a JObject
            JObject json = JObject.Parse(jsonString);

            // Access the value of the "_id" field
            string id = json["_id"].ToString();

            return id;
        }
        catch (JsonException ex)
        {
            // Handle any JSON parsing errors here
            Console.WriteLine($"_id is missing or json parsing error: {ex.Message}");
            return null;
        }
    }
}