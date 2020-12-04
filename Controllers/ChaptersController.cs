using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Flashcards.DAL;
using Flashcards.Models;

using System.IO;

using System.Text;
using CsvHelper;
using System.Web.Services.Description;
using System.Collections;
using PagedList;
using System.Drawing;

namespace Flashcards.Controllers
{

    public class ChaptersController : Controller
    {
        String hm = "," + "";   
        private FlashcardsContext db = new FlashcardsContext();

        // GET: Chapters
        public ActionResult Index()
        {
            return View(db.Chapters.ToList());
        }
        public ActionResult Statistic(int? id )
        {
            if(id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(id);
            
            
                if(chapter == null)
                {
                   return HttpNotFound();
                }
            List<Answer> answers = db.Answers.Where<Answer>(a => a.UserID == User.Identity.Name).ToList();
            foreach(var a in answers)
            {

                if(!chapter.FlashCards.Exists(b=>b.ID==a.FlashCard.ID))
                    {
                    answers.Remove(a);
                }
            }

            return View(answers);
            
        }

        // GET: Chapters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult Upload(HttpPostedFileBase postedFile, int Id)
        {
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName));
                ViewBag.Message = "File uploaded successfully.";
            }

            var path1 = Server.MapPath("~/Uploads/") + postedFile.FileName;
        var lista = CSVToList(path1);
          Chapter ch=  db.Chapters.Find(Id);
            foreach(var a in lista)
            {
                Console.WriteLine(a.Word1 + a.Word2);
                if (!ch.FlashCards.Exists(c => c.Word1 == a.Word1 && c.Word2 == a.Word1))
                {
                    ch.FlashCards.Add(a);
                }
            }
            
            db.Entry(ch).State = EntityState.Modified;
            db.SaveChanges();
            System.IO.File.Delete(path1);

            return  RedirectToAction("Details" + "/" + Id);

        }

static List<FlashCard> CSVToList(string path)
{
    var data = System.IO.File.ReadAllLines(path);
    return data.Select(m => m.Split(';')).Select(m => new FlashCard() { Word1 = m[0], Word2 = m[1] }).ToList();
}
// GET: Chapters/Create
public ActionResult Create(int id)
        {
            return View();
        }

        // POST: Chapters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                db.Chapters.Add(chapter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(chapter);
        }
        [HttpPost]
       
        public ActionResult AddFlashCard([Bind(Include = "ID,Word1,Word2,Image")] FlashCard flashCard)
        {
            int id = flashCard.ID;
            String Word1 = flashCard.Word1;

            String Word2 = flashCard.Word2;
            Chapter chapter = db.Chapters.Find(flashCard.ID);                                                                                                                                                                                                                                                                                                           
            int rc = chapter.FlashCards.FindLastIndex(a => a.ID > 0);
            rc++;
            flashCard.Word1 = Word1;
            flashCard.Word2 = Word2;
            flashCard.ID = rc;
            flashCard.UserID = User.Identity.Name;
            HttpPostedFileBase file = Request.Files["Imagefile"];
            if(file !=null && file.ContentLength > 0 )
            {
                flashCard.Image = file.FileName;
                file.SaveAs(HttpContext.Server.MapPath("~/images/") + flashCard.Image);
            }
            db.FlashCards.Add(flashCard);
            chapter.FlashCards.Add(flashCard);
            db.Entry(chapter).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details" + "/" + id);
            
        }

        // GET: Chapters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        // POST: Chapters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chapter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chapter);
        }

        // GET: Chapters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        // POST: Chapters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            db.Chapters.Remove(chapter);
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
