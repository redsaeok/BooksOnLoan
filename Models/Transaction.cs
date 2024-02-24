using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BooksOnLoan.Models;

public class Transaction
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Transaction ID")]
    public int TransactionId { get; set; }
    
    [Required]
    [Display(Name = "Book ID")]
    public int BookCodeNumber { get; set; }

    //public int TransactionTypeId { get; set; }

    public string? Username { get; set; }

    [Display(Name = "Hold Date")]
    public DateOnly HoldDate { get; set; }
    
    [Display(Name = "Loan Start Date")]
    public DateOnly? LoanStartDate { get; set; }
    
    [Display(Name = "Due Date")]
    public DateOnly? LoanDueDate { get; set; }

    [Display(Name = "Return Date")]
    public DateOnly? ReturnDate { get; set; }

    [ForeignKey("BookCodeNumber")]
    public Book? Book { get; set; }

    //[ForeignKey("TransactionTypeId")]
    //public TransactionType? TransactionType { get; set; }

}
