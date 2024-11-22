using Microsoft.EntityFrameworkCore;
using Lab2MPA.Models;

namespace Lab2MPA.Data
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new Lab2MPAContext(serviceProvider.GetRequiredService<DbContextOptions<Lab2MPAContext>>()))
            {
                if (context.Book.Any())
                {
                    return; // BD a fost creata anterior
                }
                context.Book.AddRange(
                    new Book{
                    Title = "Baltagul",
                    Price=Decimal.Parse("22")},
                    new Book{
                    Title = "Enigma Otiliei",
                    Price=Decimal.Parse("18")},
                    new Book{
                    Title = "Maytrei",
                    Price=Decimal.Parse("27")}
                    );

                context.Genre.AddRange(
                    new Genre { Name = "Roman" },
                    new Genre { Name = "Nuvela" },
                    new Genre { Name = "Poezie" }
                );

                context.Author.AddRange(
                    new Author {ID=1, FirstName= "Mihail", LastName= "Sadoveanu" },
                    new Author {ID = 2, FirstName = "George", LastName = "Calinescu" },
                    new Author {ID = 3, FirstName = "Mircea", LastName = "Eliade" }
                );

                context.Customer.AddRange(
                new Customer{
                    Name = "Popescu Marcela",
                    Adress = "Str. Plopilor, nr. 24",
                    BirthDate = DateTime.Parse("1979-09-01")
                },
                new Customer{
                    Name = "Mihailescu Cornel",
                    Adress = "Str. Bucuresti, nr.45, ap. 2",BirthDate=DateTime.Parse("1969 - 07 - 08")}
               
                );

                context.SaveChanges();
            }
        }
    }
}
