using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksOnLoan.Models;

namespace BooksOnLoan.Services;

public class QuoteOfTheDayService
{
    public string Get()
    {
        return QuoteOfTheDay.GetQuote();
    }
}
