using FiDa.Database;
using FiDa.DatabaseModels;
using FiDa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FiDa.Controllers;

public class DashboardController : Controller
{
    private readonly Account _currentUser;
    private readonly ILogger _logger;
    private readonly FiDaDatabase _db = new(new());

    public DashboardController(ILogger<DashboardController> logger, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _logger.LogInformation("New Instance DashboardController");

        var uName = contextAccessor.HttpContext?.User.Identity?.Name ?? throw new ArgumentNullException("Username");
        _currentUser = _db.Account.Include(a => a.ConfiguredHosts).FirstOrDefault(a => a.Username == uName) ?? throw new Exception($"No Account for {uName} found on database");

    }

    [Authorize]
    public IActionResult Index()
    {
        _logger.LogInformation("Dashboard - Index");

        var modle = new BaseViewModel
        {
            Account = _currentUser
        };
        return View(modle);
    }

    //[Authorize]
    public IActionResult Privacy()
    {
        _logger.LogInformation("Dashboard - Privacy");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogInformation("Dashboard - Error");

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
