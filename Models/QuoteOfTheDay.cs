using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksOnLoan.Models;

public static class QuoteOfTheDay
{
    private static string[] quotes = new string[]
    {
        "The only way to do great work is to love what you do. If you haven't found it yet, keep looking. Don't settle. As with all matters of the heart, you'll know when you find it. - Steve Jobs",
        "The only limit to our realization of tomorrow will be our doubts of today. - Franklin D. Roosevelt",
        "The best and most beautiful things in the world cannot be seen or even touched - they must be felt with the heart. - Helen Keller",
        "The best preparation for tomorrow is doing your best today. - H. Jackson Brown, Jr.",
        "The best way to predict the future is to create it. - Peter Drucker",
        "The future belongs to those who believe in the beauty of their dreams. - Eleanor Roosevelt",
        "The future depends on what you do today. - Mahatma Gandhi",
        "The future is promised to no one. - Wayne Dyer",
        "The future is what you make of it, so make it a good one. - Dr. Emmett Brown",
        "The future is yours. Do something about it. - Unknown",
        "The future's so bright, I gotta wear shades. - Timbuk 3",
        "The future's uncertain and the end is always near. - Jim Morrison"
    };

    private static string? quote;
    private static DateTime lastGenerated = DateTime.MinValue;

    private static string GenerateQuote()
    {
        Random random = new Random();
        int index = random.Next(quotes.Length);
        return quotes[index];
    }

    public static string GetQuote()
    {
        if (quote == null || DateTime.Now.Day != lastGenerated.Day)
        {
            quote = GenerateQuote();
            lastGenerated = DateTime.Now;
        }

        return quote;
    }
}
