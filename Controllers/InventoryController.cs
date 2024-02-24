using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BooksOnLoan.Data;
using BooksOnLoan.Models;
using Microsoft.AspNetCore.Authorization;

namespace BooksOnLoan.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Inventory
        public async Task<IActionResult> Index()
        {
            var query = _context.Transactions!
                .Where(t => t.ReturnDate == null)
                .GroupBy(t => t.BookCodeNumber)                
                .Select(g => new
                {
                    Book = g.Key,
                    TransactionCount = g.Count()
                });

            int overdueCount = _context.Transactions!
                .Where(t => t.ReturnDate == null)
                .Where(t => t.LoanDueDate < DateOnly.FromDateTime(DateTime.Now))
                .Where(t => t.Username == User.Identity!.Name)
                .Count();

            ViewBag.OverdueCount = overdueCount;

            Dictionary<int, int> bookTransactionCounts = new Dictionary<int, int>();
            foreach (var item in query)
            {
                bookTransactionCounts.Add(item.Book, item.TransactionCount);
            }

            ViewBag.BookTransactionCounts = bookTransactionCounts;

            return View(await _context.Inventory!.ToListAsync());
        }

        [Authorize]
        // GET: Inventory/Details/5
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

            var book = await _context.Inventory!
                .FirstOrDefaultAsync(m => m.CodeNumber == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize]
        // GET: Inventory/Create
        public IActionResult Create()
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }



            return View();
        }

        // POST: Inventory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodeNumber,Title,Author,YearPublished,Quantity")] Book book)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }



            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Inventory/Edit/5
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

            var book = await _context.Inventory!.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Inventory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodeNumber,Title,Author,YearPublished,Quantity")] Book book)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }



            if (id != book.CodeNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.CodeNumber))
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
            return View(book);
        }

        // GET: Inventory/Delete/5
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

            var book = await _context.Inventory!
                .FirstOrDefaultAsync(m => m.CodeNumber == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.IsInRole("Admin") is false)
            {
                return NotFound();
            }



            var book = await _context.Inventory!.FindAsync(id);
            if (book != null)
            {
                _context.Inventory.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Inventory!.Any(e => e.CodeNumber == id);
        }
    }
}
