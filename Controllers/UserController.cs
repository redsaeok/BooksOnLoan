using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BooksOnLoan.Models;
using Microsoft.AspNetCore.Identity;

namespace BooksOnLoan.Controllers;

public class UserController : Controller
{
    private UserManager<CustomUser> userManager;

    public UserController(UserManager<CustomUser> userMgr)
    {
        userManager = userMgr;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult List()
    {
        ViewData["userManager"] = userManager;
        return View(userManager.Users);
    }


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
