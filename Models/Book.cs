using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BooksOnLoan.Models;

public class Book
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Book ID")]
    public int CodeNumber { get; set; }
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Author { get; set; }    
    [Required]
    [Display(Name = "Year Published")]
    public int YearPublished { get; set; }
    [Required]
    public int Quantity { get; set; }    

    public List<Transaction>? Transactions { get; set; }
}
