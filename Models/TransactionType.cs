using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BooksOnLoan.Models
{
    public class TransactionType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TransactionTypeID { get; set; }
        public string? Description { get; set; }
    }
}