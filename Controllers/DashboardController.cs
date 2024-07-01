using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FiDa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using FiDa.DatabaseModels;
using FiDa.Lib;

namespace FiDa.Controllers;

public class DashboardController : Controller
{
    private readonly Account _currentUser;
    private readonly ILogger _logger;

    public DashboardController(ILogger<DashboardController> logger, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _currentUser = Utils.GetAccount(contextAccessor.HttpContext?.User.Identity?.Name!, includeHosts: true);

        _logger.LogInformation("New Instance DashboardController");
    }

    [Authorize]
    public IActionResult Index()
    {
        _logger.LogInformation("Dashboard Index");

        _logger.LogDebug("_currentUser: {username}", _currentUser.Username);
        _logger.LogDebug("hosts: {username}", string.Join(", ", _currentUser.ConfiguredHosts));
        var modle = new BaseViewModel
        {
            Account = _currentUser
        };
        return View(modle);
    }

    //[Authorize]
    public IActionResult Privacy()
    {
        _logger.LogInformation("Dashboard Privacy");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogInformation("Dashboard Error");

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
