using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BooksOnLoan.Data;
using BooksOnLoan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using Microsoft.AspNetCore.Authorization;

namespace BooksOnLoan.Controllers
{
    [Route("[controller]")]
    public class OverdueEmailController : Controller
    {

        private readonly ApplicationDbContext _context;

        public OverdueEmailController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet    ("{id:int}")]
        public IActionResult SendOverdueNotice(int id)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }



            ViewData["TransactionId"] = id;
            ViewData["Sender"] = User.Identity!.Name;
            ViewData["Recipient"] = _context.Transactions!.Find(id)!.Username;
            ViewData["Subject"] = $"Overdue Notice for {_context.Transactions!.Where( x => x.TransactionId == id).Include(t => t.Book).FirstOrDefault()!.Book!.Title}";

            return View();
        }

        [Authorize]
        [HttpPost, ActionName("SendOverdueNotice")]
        [ValidateAntiForgeryToken]
        public IActionResult SendOverdueNotice(int id, [Bind("From,To,Subject,Body")] Email email)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }



            if (ModelState.IsValid)
            {
                // redirect to the index action of the transaction controller
                return RedirectToAction(nameof(Index), nameof(Transaction));
            }

            ViewData["TransactionId"] = id;
            ViewData["Sender"] = User.Identity!.Name;
            ViewData["Recipient"] = _context.Transactions!.Find(id)!.Username;
            ViewData["Subject"] = email.Subject;
            ViewData["Body"] = email.Body;

            ViewBag.ErrorMessage = "Invalid email data";
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
    
}