using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Flashcards.DAL;
using Flashcards.Models;
using PagedList;
namespace Flashcards.Controllers
{
    public class CoursesController : Controller
    {
        private FlashcardsContext db = new FlashcardsContext();

        // GET: Courses
        [Authorize]
        public ViewResult Index(string searchString, int? page, string currentFilter)
        {   
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var courses = from s in db.Courses 
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.Name.Contains(searchString));
                                       
            }
            courses  = courses .OrderBy(s => s.Name);
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            return View(courses.ToPagedList(pageNumber, pageSize));
            
        }

        // GET: Courses/Details/5
        public ViewResult Details(int? id, string searchString, int? page, string currentFilter)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            
          
            }
            ViewBag.CurrentFilter = searchString;
            var Course = db.Courses.Where(i => i.ID == id).First();
            ViewData["UserId"] = Course.IDUSER.ToString();
            ViewData["id"] = Course.ID.ToString();

            IEnumerable<Chapter> chapters = Course.Chapters.ToList();
            
            if (!String.IsNullOrEmpty(searchString))
            {
                chapters = chapters.Where(a=>a.Name.Contains(searchString));

            }
            chapters = chapters.ToList().OrderBy(s => s.Name);
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            return View(chapters.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult  AddChapter (string Id, string Name)
        {
            Course course = db.Courses.Find(Int32.Parse(Id));
            int rc = course.Chapters.FindLastIndex(a => a.ID > 0);
            rc++;
            course.Chapters.Add(new Chapter
            {
                ID = rc,
                Name = Name,
                UserID = User.Identity.Name

            }); ; 
            db.Entry(course).State = EntityState.Modified;
            db.SaveChanges();
           return RedirectToAction("Details" + "/" + Id);

        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Course course)
        {
            if (ModelState.IsValid)
            {
                course.IDUSER = User.Identity.Name;
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            course.Chapters.Clear();
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
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
