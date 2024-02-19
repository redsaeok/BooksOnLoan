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
    public int TransactionId { get; set; }
    
    public int BookCodeNumber { get; set; }

    //public int TransactionTypeId { get; set; }

    public string? Username { get; set; }

    public DateOnly HoldDate { get; set; }
    
    public DateOnly? LoanStartDate { get; set; }
    
    public DateOnly? LoanDueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    [ForeignKey("BookCodeNumber")]
    public Book? Book { get; set; }

    //[ForeignKey("TransactionTypeId")]
    //public TransactionType? TransactionType { get; set; }

}
