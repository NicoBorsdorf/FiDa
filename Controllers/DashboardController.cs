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

    [Authorize]
    public IActionResult Index()
    {
        string? uName = User.Identity?.Name;
        if (uName == null) throw new Exception("No Username for given user.");

        var _currentUser = _db.Account.FirstOrDefault((a) => a.Username == uName);

        if (_currentUser == null) _currentUser = Utils.AddAccount(new Account(uName)).Result;
        if (_currentUser == null) throw new Exception("Account creation failed.");

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
