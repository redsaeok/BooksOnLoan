using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BooksOnLoan.Models;
using Microsoft.AspNetCore.Identity;

namespace BooksOnLoan.Controllers;

public class RoleController : Controller
{
    private RoleManager<CustomRole> roleManager;

    public RoleController(RoleManager<CustomRole> roleMgr)
    {
        roleManager = roleMgr;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult List()
    {
        return View(roleManager.Roles);
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
