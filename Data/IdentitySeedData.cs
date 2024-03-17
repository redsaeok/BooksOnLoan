using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksOnLoan.Models;
using Microsoft.AspNetCore.Identity;

namespace BooksOnLoan.Data;

public class IdentitySeedData
{
    
    public static async Task Initialize(ApplicationDbContext context,
        UserManager<CustomUser> userManager,
        RoleManager<CustomRole> roleManager)
    {
        context.Database.EnsureCreated();

        string adminRole = "Admin";
        string memberRole = "Member";
        string password4all = "P@$$w0rd";

        if (await roleManager.FindByNameAsync(adminRole) == null)
        {
            await roleManager.CreateAsync(new CustomRole(adminRole));
        }

        if (await roleManager.FindByNameAsync(memberRole) == null)
        {
            await roleManager.CreateAsync(new CustomRole(memberRole));
        }

        if (await userManager.FindByNameAsync("aa@aa.aa") == null)
        {
            var user = new CustomUser
            {
                UserName = "aa@aa.aa",
                Email = "aa@aa.aa",                
                MobileNumber = "6902341234",
                FirstName = "Alice",
                LastName = "Adams",
                StreetAddressOne = "123 Main St",
                StreetAddressTwo = "",
                City = "Vancouver",
                Province = "BC",
                PostalCode = "V6A 1A1"                
            };

            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await userManager.AddPasswordAsync(user, password4all);
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }

        if (await userManager.FindByNameAsync("mm@mm.mm") == null)
        {
            var user = new CustomUser
            {
                UserName = "mm@mm.mm",
                Email = "mm@mm.mm",                
                MobileNumber = "6572136821",
                FirstName = "Maggie",
                LastName = "Mason",
                StreetAddressOne = "456 Main St",
                StreetAddressTwo = "",
                City = "Vancouver",
                Province = "BC",
                PostalCode = "V6A 1A1"
            };

            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await userManager.AddPasswordAsync(user, password4all);
                await userManager.AddToRoleAsync(user, memberRole);
            }
        }

    }
    
}