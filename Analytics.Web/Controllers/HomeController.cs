using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Analytics.Web.Models;
using Analytics.Web.Services;
using Newtonsoft.Json;

namespace Analytics.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly RedisService _redisService;
    
    public HomeController(ILogger<HomeController> logger, RedisService redisService)
    {
        _logger = logger;
        _redisService = redisService;
    }

    public IActionResult Index()
    {
        string jsonString = _redisService.GetValueFromDatabase("funny:top1");
        var socialPost = JsonConvert.DeserializeObject<SocialPost>(jsonString);

        var homeModel = new HomeModel();
        homeModel.SocialPost = socialPost;
            
        List<string> jsonData = _redisService.GetPatternFromDatabase("sports:*");
        var sportsPosts = new List<SportsPost>();
        
        foreach (var json in jsonData)
        {
            var sportsPost = JsonConvert.DeserializeObject<SportsPost>(json);
            sportsPosts.Add(sportsPost);
        }

        homeModel.SportsPosts = sportsPosts;
            
        return View(homeModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}