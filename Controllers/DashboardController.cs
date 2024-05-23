using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FiDa.Models;
using Microsoft.AspNetCore.Authorization;
using FiDa.Database;
using FiDa.DatabaseModels;
using FiDa.Lib;

namespace FiDa.Controllers;

public class DashboardController : Controller
{
    private readonly FiDaDatabase _db = new();
    private Account _currentUser;

    public DashboardController(IHttpContextAccessor contextAccessor)
    {
        _currentUser = Utils.GetAccount(contextAccessor.HttpContext?.User.Identity?.Name);
    }

    [Authorize]
    public IActionResult Index()
    {
        return View(_currentUser);
    }

    //[Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
