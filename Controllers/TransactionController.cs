using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BooksOnLoan.Data;
using BooksOnLoan.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
//using MimeKit;
//using MailKit.Net.Smtp;
//using MailKit;

namespace BooksOnLoan.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        [Authorize]
        public async Task<IActionResult> Index()
        {
            int overdueCount = _context.Transactions!
                .Where(t => t.ReturnDate == null)
                .Where(t => t.LoanDueDate < DateOnly.FromDateTime(DateTime.Now))
                .Where(t => t.Username == User.Identity!.Name)
                .Count();

            ViewBag.OverdueCount = overdueCount;

            // books waiting to be sent out
            var allBooks =
                _context.Transactions!
                    .Include(t => t.Book);

            var memberReservations =
                _context.Transactions!
                    .Include(t => t.Book)
                    .Where(t => t.Username == User.Identity!.Name)
                    .Where(t => t.ReturnDate.HasValue == false)
                    .Where(t => t.LoanStartDate.HasValue == false);

            var memberHasBooks =
                _context.Transactions!
                    .Include(t => t.Book)
                    .Where(t => t.Username == User.Identity!.Name)
                    .Where(t => t.ReturnDate.HasValue == false)
                    .Where(t => t.LoanStartDate.HasValue == true);

            // books waiting to be sent out
            var heldBooks =
                _context.Transactions!
                    .Include(t => t.Book)
                    .Where(t => t.ReturnDate.HasValue == false)
                    .Where(t => t.LoanStartDate.HasValue == false);


            // books waiting to be returned
            var sentBooks =
                _context.Transactions!
                    .Include(t => t.Book)
                    .Where(t => t.ReturnDate.HasValue == false)
                    .Where(t => t.LoanStartDate.HasValue == true);

            // overdueBooks
            var overdueBooks =
                _context.Transactions!
                    .Include(t => t.Book)
                    .Where(t => t.ReturnDate.HasValue == false)
                    .Where(t => t.LoanStartDate.HasValue == true)
                    .Where(t => t.LoanDueDate < @DateOnly.FromDateTime(DateTime.Now));

            ViewData["MemberReserveBooks"] = await memberReservations.ToListAsync();
            ViewData["MemberHasBooks"] = await memberHasBooks.ToListAsync();
            ViewData["HeldBooks"] = await heldBooks.ToListAsync();
            ViewData["SentBooks"] = await sentBooks.ToListAsync();
            ViewData["OverdueBooks"] = await overdueBooks.ToListAsync();

            return View(await allBooks.ToListAsync());
        }

        // GET: Transaction/SendEmail
        [Authorize]
        public ActionResult SendEmail()
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }


            MailAddress to = new MailAddress("johnglenn@shaw.ca");
            MailAddress from = new MailAddress("books.on.loan.jg@gmail.com");

            MailMessage email = new MailMessage(from, to);
            email.Subject = "Testing out email sending";
            email.Body = "Hello all the way from the land of C#";

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 25;
            smtp.Credentials = new NetworkCredential("books.on.loan.jg@gmail.com", "[Redacted]");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;

            try
            {
                /* Send method called below is what will send off our email 
                 * unless an exception is thrown.
                 */
                smtp.Send(email);
            }
            catch (SmtpException ex)
            {
                ViewData["Message"] = $"Error sending email: {ex.Message}";
            }

            return View();
        }


        // GET: Transaction/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }


            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions!
                .Include(t => t.Book)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Reserve/5
        [Authorize]
        public IActionResult Reserve(int? id)
        {

            if (User.IsInRole("Member") is false)
            {
                return NotFound();
            }


            if (id is null)
            {
                return NotFound();
            }

            if (_context.Inventory!.Find(id) is null)
            {
                return NotFound();
            }

            var book = _context.Inventory.Find(id);

            ViewData["BookCodeNumber"] = new SelectList(_context.Inventory, "CodeNumber", "Title");
            ViewData["Book"] = book;
            ViewData["UserID"] = User.Identity?.Name!;

            return View();
        }


        // POST: Transaction/Reserve
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Reserve([Bind("TransactionId,BookCodeNumber,Username,HoldDate,LoanStartDate,LoanDueDate,ReturnDate")] Transaction transaction)
        {
            if (User.IsInRole("Member") is false)
            {
                return NotFound();
            }


            if (ModelState.IsValid && transaction.HoldDate >= DateOnly.FromDateTime(DateTime.Now))
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (transaction is null || _context.Inventory!.Find(transaction.BookCodeNumber) is null)
            {
                return NotFound();
            }

            var book = _context.Inventory.Find(transaction.BookCodeNumber);

            ViewData["Message"] = "Reserve failed.  Reservation date must be today or in the future.  Please try again.";
            ViewData["BookCodeNumber"] = new SelectList(_context.Inventory, "CodeNumber", "Title", transaction.BookCodeNumber);
            ViewData["Book"] = book;
            ViewData["UserID"] = User.Identity?.Name!;
            return View(transaction);
        }


        // GET: Transaction/Create
        [Authorize]
        public IActionResult Create()
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }


            ViewData["BookCodeNumber"] = new SelectList(_context.Inventory, "CodeNumber", "Title");
            return View();
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("TransactionId,BookCodeNumber,Username,HoldDate,LoanStartDate,LoanDueDate,ReturnDate")] Transaction transaction)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookCodeNumber"] = new SelectList(_context.Inventory, "CodeNumber", "Title", transaction.BookCodeNumber);
            return View(transaction);
        }

        // GET: Transaction/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }

            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions!.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["BookCodeNumber"] = new SelectList(_context.Inventory, "CodeNumber", "Title", transaction.BookCodeNumber);
            return View(transaction);
        }


        // GET: Transaction/Deliver/5
        [Authorize]
        public async Task<IActionResult> Deliver(int? id)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }

            if (User is null || User.Identity is null || User.Identity.IsAuthenticated is false)
            {
                return RedirectToPage("/Account/Login");
            }

            if (id is null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions!.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            if (_context.Inventory!.Find(transaction.BookCodeNumber) is null)
            {
                return NotFound();
            }

            var book = _context.Inventory.Find(transaction.BookCodeNumber);

            ViewData["TransactionID"] = transaction.TransactionId;
            ViewData["BookCodeNumber"] = transaction.BookCodeNumber;
            ViewData["Book"] = book;

            return View(transaction);
        }


        // GET: Transaction/Return/5
        [Authorize]
        public async Task<IActionResult> Return(int? id)
        {
            // only allow members or admins to return books
            if (User.IsInRole("Member") is false && User.IsInRole("Admin") is false)
            {
                return NotFound();
            }


            if (User is null || User.Identity is null || User.Identity.IsAuthenticated is false)
            {
                return RedirectToPage("/Account/Login");
            }

            if (id is null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions!.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            if (_context.Inventory!.Find(transaction.BookCodeNumber) is null)
            {
                return NotFound();
            }

            var book = _context.Inventory.Find(transaction.BookCodeNumber);

            // only allow admins or the member who has the book to return it
            if( !User.IsInRole("Admin") && User.IsInRole("Member") && transaction.Username != User.Identity.Name)
            {
                return NotFound();
            }

            ViewData["TransactionID"] = transaction.TransactionId;
            ViewData["BookCodeNumber"] = transaction.BookCodeNumber;
            ViewData["Book"] = book;

            return View(transaction);
        }


        // POST: Transaction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,BookCodeNumber,Username,HoldDate,LoanStartDate,LoanDueDate,ReturnDate")] Transaction transaction)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }

            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookCodeNumber"] = new SelectList(_context.Inventory, "CodeNumber", "Title", transaction.BookCodeNumber);
            return View(transaction);
        }


        // POST: Transaction/Deliver/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Deliver(int id, [Bind("TransactionId,BookCodeNumber,Username,HoldDate,LoanStartDate,LoanDueDate,ReturnDate")] Transaction transaction)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }

            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid && transaction.LoanStartDate >= transaction.HoldDate && transaction.LoanDueDate > transaction.LoanStartDate)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var book = _context.Inventory!.Find(transaction.BookCodeNumber);
            ViewData["Message"] = $"Delivery failed.  Loan start date must be greater than or equal to the hold date of {transaction.HoldDate}" +
                $"and loan due date must be after the loan start date of {transaction.LoanStartDate}.  Please try again.";
            ViewData["TransactionID"] = transaction.TransactionId;
            ViewData["BookCodeNumber"] = transaction.BookCodeNumber;
            ViewData["Book"] = book;
            return View(transaction);
        }


        // POST: Transaction/Return/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Return(int id, [Bind("TransactionId,BookCodeNumber,Username,HoldDate,LoanStartDate,LoanDueDate,ReturnDate")] Transaction transaction)
        {
            if (User.IsInRole("Member") is false)
            {
                return NotFound();
            }

            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid && transaction.ReturnDate >= transaction.LoanStartDate)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var book = _context.Inventory!.Find(transaction.BookCodeNumber);
            ViewData["TransactionID"] = transaction.TransactionId;
            ViewData["BookCodeNumber"] = transaction.BookCodeNumber;
            ViewData["Book"] = book;
            ViewData["Message"] = $"Return failed.  Return date must be greater than or equal to the loan start date of {transaction.LoanStartDate.ToString()}.  Please try again.";
            ViewData["BookCodeNumber"] = new SelectList(_context.Inventory, "CodeNumber", "Title", transaction.BookCodeNumber);
            return View(transaction);
        }


        // GET: Transaction/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }

            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions!
                .Include(t => t.Book)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions!.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions!.Any(e => e.TransactionId == id);
        }



    }
}
