using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BooksOnLoan.Models;
using Microsoft.AspNetCore.Identity;

namespace BooksOnLoan.Controllers;

public class UserPermissionController : Controller
{
    private UserManager<CustomUser> userManager;

    public UserPermissionController(UserManager<CustomUser> userMgr)
    {
        userManager = userMgr;
    }

    public IActionResult List()
    {
        ViewData["userManager"] = userManager;
        return View(userManager.Users);
    }

    // GET: User/Edit/{id}
    public async Task<IActionResult> Edit(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        var userPermissions = new UserPermissions
        {
            Id = user.Id,
            Email = user.Email,
            IsAdmin = await userManager.IsInRoleAsync(user, "Admin"),
            IsMember = await userManager.IsInRoleAsync(user, "Member")
        };

        return View(userPermissions);
    }

    // POST: User/Edit/{id}
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Email,IsAdmin,IsMember")] UserPermissions userPermissions)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        if( userPermissions.IsAdmin )
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            await userManager.RemoveFromRoleAsync(user, "Admin");
        }

        if( userPermissions.IsMember )
        {
            await userManager.AddToRoleAsync(user, "Member");
        }
        else
        {
            await userManager.RemoveFromRoleAsync(user, "Member");
        }

        return View(userPermissions);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
