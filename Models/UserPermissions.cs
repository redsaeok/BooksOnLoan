using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BooksOnLoan.Models;

public class UserPermissions
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsMember { get; set; }
}
