using Library.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library.Repository.Data
{
    public static class LibraryContextSeed
    {
        public static async Task SeedAsync(LibraryContext _dbContext)
        {
            if (_dbContext.Categories.Count()==0)
            {
                var categoriesData = File.ReadAllText("../Library.Repository/Data/DataSeed/categories.json");
                var categories = JsonSerializer.Deserialize<List<Category>>(categoriesData);
                if (categories?.Count()>0)
                {
                    foreach (var category in categories)
                    {
                        _dbContext.Categories.Add(category);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            if (_dbContext.Books.Count() == 0)
            {
                var booksData = File.ReadAllText("../Library.Repository/Data/DataSeed/books.json");
                var Books = JsonSerializer.Deserialize<List<Book>>(booksData);
                if (Books?.Count() > 0)
                {
                    foreach (var book in Books)
                    {
                        _dbContext.Books.Add(book);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
