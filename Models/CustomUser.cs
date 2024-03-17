using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BooksOnLoan.Models;

public class CustomUser : IdentityUser
{
    public CustomUser() : base() { }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public string? MobileNumber { get; set; }
    
    public string? StreetAddressOne { get; set; }
    public string? StreetAddressTwo { get; set; }
    
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    
}