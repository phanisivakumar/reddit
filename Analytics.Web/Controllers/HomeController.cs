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
        

        return View(socialPost);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}