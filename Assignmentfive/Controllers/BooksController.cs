using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Assignmentfive.Models;

namespace Assignmentfive.Controllers
{
    public class BooksController : Controller
    {
        private Assignment5Entities db = new Assignment5Entities();

        // GET: Books
        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.Category);
            return View(books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Cat_ID = new SelectList(db.Categories, "CategoryID", "C_Name");
            return View();
        }
        [HttpPost]
        public ActionResult Create(Book img)
        {
            string fileName = Path.GetFileNameWithoutExtension(img.ImageFile.FileName);
            string extension = Path.GetExtension(img.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            img.B_Image = "../Images/" + fileName;
            fileName = Path.Combine(Server.MapPath("../Images/"), fileName);
            img.ImageFile.SaveAs(fileName);
            using (Assignment5Entities entities = new Assignment5Entities())
            {
                entities.Books.Add(img);
                entities.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Customer")]
        public ActionResult Buy(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }
        public ActionResult OrderSummary()
        {
            ViewBag.name = User.Identity.Name;
            return View(db.OrderSummaries.ToList());
        }
        [HttpPost]
        public ActionResult OrderSummary(FormCollection fc, string quantity, string B_Name)
        {
            int id = Convert.ToInt32(fc["BookID"]);
            Book book = db.Books.Find(id);
            book.Quantity = book.Quantity - Convert.ToInt32(fc["quantity1"]);
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();
            //using (Assignment5Entities1 entities1 = new Assignment5Entities1())
            //{



            // entities1.Books.Add(book1);
            // entities1.SaveChanges();



            //}
            OrderSummary os = new OrderSummary();
            os.O_Name = fc["UserName"];
            os.B_id = Convert.ToInt32(fc["BookID"]);
            os.Amount = Convert.ToInt32(fc["totalprice"]);
            os.Qty = Convert.ToInt32(fc["quantity1"]);
            os.BookName = fc["B_Name"];
            using (Assignment5Entities entities = new Assignment5Entities())
            {
                entities.OrderSummaries.Add(os);
                //entities.Books.Add(book);
                entities.SaveChanges();
            }
            ViewBag.name = fc["UserName"];
            return View(db.OrderSummaries.ToList());
        }



        [Authorize(Roles = "Admin")]
        public ActionResult AllOrders()
        {
            return View(db.OrderSummaries.ToList());
        }
        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "BookID,B_Name,Author,Price,Quantity,B_Image,Cat_ID")] Book book)
        //{
        // if (ModelState.IsValid)
        // {
        // db.Books.Add(book);
        // db.SaveChanges();
        // return RedirectToAction("Index");
        // }



        // ViewBag.Cat_ID = new SelectList(db.Categories, "CategoryID", "C_Name", book.Cat_ID);
        // return View(book);
        //}

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.Cat_ID = new SelectList(db.Categories, "CategoryID", "C_Name", book.Cat_ID);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookID,B_Name,Author,Price,Quantity,B_Image,Cat_ID")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Cat_ID = new SelectList(db.Categories, "CategoryID", "C_Name", book.Cat_ID);
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Shows(string search)
        {
            ViewData["Books"] = search;
            List<Book> book = new List<Book>();
            var books = (from b in db.Books where b.B_Name == search || b.Author == search select b);



            foreach (var b in books)
            {
                book.Add(b);
            }
            return View(book);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}