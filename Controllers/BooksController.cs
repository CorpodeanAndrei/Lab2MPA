using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2MPA.Data;
using Lab2MPA.Models;

namespace Lab2MPA.Controllers
{
    public class BooksController : Controller
    {
        private readonly Lab2MPAContext _context;

        public BooksController(Lab2MPAContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var books = from b in _context.Book
                        join a in _context.Author on b.AuthorID equals a.ID
                        select new BookViewModel
                        {
                            ID = b.ID,
                            Title = b.Title,
                            Price = b.Price,
                            FullName = a.FullName
                        };

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    books = books.OrderByDescending(b => b.Title);
                    Console.WriteLine(books);
                    break;
                case "Price":
                    books = books.OrderBy(b => b.Price);
                    break;
                case "price_desc":
                    books = books.OrderByDescending(b => b.Price);
                    break;
                default:
                    books = books.OrderBy(b => b.Title);
                    break;
            }
            int pageSize = 2;
            return View(await PaginatedList<BookViewModel>.CreateAsync(books.AsNoTracking(),pageNumber ?? 1, pageSize));
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)  // Include autorul, dar nu și cărțile acestuia
                .Include(b => b.Orders)
                    .ThenInclude(o => o.Customer)  // Include comenzile și clienții
                .AsNoTracking()  // Nu urmări obiectele
                .FirstOrDefaultAsync(b => b.ID == id);

            if (book == null)
            {
                return NotFound();
            }

            // Obține toate cărțile autorului separat
            var authorBooks = await _context.Book
                .Where(b => b.AuthorID == book.AuthorID)
                .AsNoTracking()  // Continuă să folosești AsNoTracking dacă e necesar
                .ToListAsync();

            // Atribuie cărțile autorului la modelul de view
            book.Author.Books = authorBooks;

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["AuthorID"] = new SelectList(_context.Author, "ID", "FullName");
            ViewData["GenreID"] = new SelectList(_context.Genre, "ID", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,AuthorID,Price,GenreID")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex*/)
            {

                ModelState.AddModelError("", "Unable to save changes. " + "Try again, and if the problem persists ");
            }

            ViewData["AuthorID"] = new SelectList(_context.Author, "ID", "FullName", book.AuthorID);
            ViewData["GenreID"] = new SelectList(_context.Genre, "ID", "Name", book.GenreID);

            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorID"] = new SelectList(_context.Author.Select(a => new
            {
                ID = a.ID,
                FullName = a.FirstName + " " + a.LastName
            }), "ID", "FullName", book.AuthorID);
            ViewData["GenreID"] = new SelectList(_context.Genre, "ID", "Name", book.GenreID);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bookToUpdate = await _context.Book.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Book>(bookToUpdate, "",  s => s.AuthorID, s => s.Title, s => s.Price))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists");
                }
            }
            ViewData["AuthorID"] = new SelectList(_context.Author, "ID", "FullName",
           bookToUpdate.AuthorID);
            return View(bookToUpdate);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (book == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                "Delete failed. Try again";
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            if (book == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Book.Remove(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {

                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.ID == id);
        }
    }
}
