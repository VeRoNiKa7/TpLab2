using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using lab2.Models;

namespace lab2.Controllers
{
    public class HomeController : Controller
    {
        BookContext db = new BookContext();
        public ActionResult Index(int? author, string genre)
        {
            IQueryable<Book> books = db.Books.Include(p => p.Author);
            if (author != null && author != 0)
            {
                books = books.Where(p => p.AuthorId == author);
            }
            if (!String.IsNullOrEmpty(genre) && !genre.Equals("Все"))
            {
                books = books.Where(p => p.Genre == genre);
            }

            List<Author> authors = db.Authors.ToList();
            // устанавливаем начальный элемент, который позволит выбрать всех
            authors.Insert(0, new Author { FIO = "Все", Id = 0 });

            BooksListViewModel blvm = new BooksListViewModel
            {
                Books = books.ToList(),
                Authors = new SelectList(authors, "Id", "FIO"),
                Genres = new SelectList(new List<string>()
            {
                "Все",
                "роман",
                "фантастика",
                "комедия",
                "детектив"
            })
            };
            return View(blvm);
        }
        //добавление авторов
        [HttpGet]
        public ActionResult Create_author()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create_author(Author author)
        {
            db.Authors.Add(author);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        //Создание новой записи
        [HttpGet]
        public ActionResult Create()
        {
            // Формируем список авторов для передачи в представление
            SelectList authors = new SelectList(db.Authors, "Id", "FIO");
            ViewBag.Authors = authors;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Book book)
        {
            //Добавляем книгу в таблицу
            db.Books.Add(book);
            db.SaveChanges();
            // перенаправляем на главную страницу
            return RedirectToAction("Index");
        }

        //Редактирование записи
        [HttpGet]
        public ActionResult EditBook(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            // Находим в бд книгу
            Book book = db.Books.Find(id);
            if (book != null)
            {
                // Создаем список авторов для передачи в представление
                SelectList authors = new SelectList(db.Authors, "Id", "FIO", book.AuthorId);
                ViewBag.Authors = authors;
                return View(book);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult EditBook(Book book)
        {
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //удаление записи
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Book b = db.Books.Find(id);
            if (b == null)
            {
                return HttpNotFound();
            }
            return View(b);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Book b = db.Books.Find(id);
            if (b == null)
            {
                return HttpNotFound();
            }
            db.Books.Remove(b);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Удаление автора
        [HttpGet]
        public ActionResult Delete_author()
        {
            // Формируем список авторов для передачи в представление
            SelectList authors = new SelectList(db.Authors, "Id", "FIO");
            ViewBag.Authors = authors;
            return View();
        }

        [HttpPost, ActionName("Delete_author")]
        public ActionResult DeleteConfirmed_author(Author author)
        {
            Author a = db.Authors.Find(author.Id);
            if (a == null)
            {
                return HttpNotFound();
            }
            db.Authors.Remove(a);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult BookView(int id)
         {
            Book b = db.Books.Find(id);
            return View(b);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}