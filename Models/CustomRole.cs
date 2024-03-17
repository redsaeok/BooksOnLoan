using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BooksOnLoan.Models;

// I'm not sure this is really needed for the Assignment
// we can try removing it and replacing references with just IdentityRole

public class CustomRole : IdentityRole
{

    public CustomRole() : base() { }

    public CustomRole(string roleName) : base(roleName) { }

    public CustomRole(string roleName, string description,
      DateTime createdDate)
      : base(roleName)
    {
        base.Name = roleName;

        this.Description = description;
        this.CreatedDate = createdDate;
    }

    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}