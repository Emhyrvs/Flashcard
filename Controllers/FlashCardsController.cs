using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Flashcards.DAL;
using Flashcards.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using PagedList;
using WebGrease.Css.Extensions;

namespace Flashcards.Controllers
{
   
    public class FlashCardsController : Controller
    {
        private static Random rng = new Random();
        private FlashcardsContext db = new FlashcardsContext();


        // GET: FlashCards
        [Authorize]
        
        public ActionResult Index(int? page,int? nr, int? know)
        {
            Random rnd = new Random();
         
            var Chapter = db.Chapters.Find(nr);
            var flashCards = Chapter.FlashCards.ToList();
            int pageSize = 1;
            
            int pageNumber = page.GetValueOrDefault(0);
               ViewData["nr"] = nr;
            pageNumber--;
            if (know == null)
            {
                String userid = User.Identity.GetUserName();
                List<Profile> profile = db.Profiles.ToList();
                Profile profil = profile.Find(x => x.UserName == userid);
                Test test = new Test();
                db.Tests.Add(test);
             
                db.SaveChanges();
                profil.Tests.Add(test);
                pageNumber = rnd.Next(flashCards.Count );
             
                db.Entry(profil).State = EntityState.Modified;
                db.SaveChanges();
              
               

            }
            else if (know == 0)
            {
                String userid = User.Identity.GetUserName();
                List<Profile> profile = db.Profiles.ToList();
                Profile profil = profile.Find(x => x.UserName == userid);
                List<Test> tests = db.Tests.ToList();
                var test = tests.OrderByDescending(a => a.ID).First();

                int nrq = test.Questions.Count + 1;
                test.Questions.Add(new Question { ID = nrq, flashCard = flashCards[pageNumber], Score = 0 });
                List<Answer> answers = db.Answers.Where(a => a.UserID == User.Identity.Name).ToList();
                Answer answer = answers.Find(a => a.FlashCard.ID == flashCards[pageNumber].ID);
                if (answer == null)
                {
                    if (db.Answers.Count() == 0)
                    {
                        Answer answer1 = new Answer { ID = 0, FlashCard = flashCards[pageNumber], UserID = User.Identity.Name, CorrectAnswers = 1 };
                        db.Answers.Add(answer1);
                    }
                    else
                    {
                        int a = db.Answers.OrderByDescending(b => b.ID).Select(b => b.ID).First();
                        Answer answer1 = new Answer { ID = a, FlashCard = flashCards[pageNumber], UserID = User.Identity.Name, CorrectAnswers = 1 };
                        db.Answers.Add(answer1);
                    }
                }
                else
                {
                    answer.CorrectAnswers = 0;
                        db.Entry(answer).State = EntityState.Modified;
                }
             
                profil.Tests.Add(test);

                

                db.Entry(profil).State = EntityState.Modified;


                db.Entry(test).State = EntityState.Modified;
                    db.SaveChanges();

                

              

             
                List<int> ids = new List<int>();
                foreach (var a in test.Questions.Where(b => b.Score == 1))
                {
                    ids.Add(flashCards.IndexOf(a.flashCard));


                }

                int i = 0;
                do
                {
                    i++;
                    pageNumber = rnd.Next(flashCards.Count);

                }
                while (ids.Contains(pageNumber) && i < 100);
            }

            else if (know == 1)
            {

                String userid = User.Identity.GetUserName();
                List<Profile> profile = db.Profiles.ToList();
                Profile profil = profile.Find(x => x.UserName == userid);
                List<Test> tests = db.Tests.ToList();
               
                var test = tests.OrderByDescending(a=>a.ID).First();

                int nrq = test.Questions.Count + 1;
                Console.WriteLine(flashCards[pageNumber].Word1+" "+flashCards[pageNumber].Word2);
                Question question = new Question { ID = nrq, flashCard = flashCards[pageNumber], Score = 1 };
              
                 List<Answer> answers = db.Answers.Where(a => a.UserID == User.Identity.Name ).ToList();
                Answer answer = answers.Find(a => a.FlashCard.ID == flashCards[pageNumber].ID);
                if (answer == null)
                {
                    if (db.Answers.Count() == 0)
                    {
                        Answer answer1 = new Answer { ID = 0, FlashCard = flashCards[pageNumber], UserID = User.Identity.Name, CorrectAnswers = 1 };
                        db.Answers.Add(answer1);
                    }
                    else
                    {
                        int a = db.Answers.OrderByDescending(b => b.ID).Select(b => b.ID).First();
                        Answer answer1 = new Answer { ID = a, FlashCard = flashCards[pageNumber], UserID = User.Identity.Name, CorrectAnswers = 1 };
                        db.Answers.Add(answer1);
                    }
                }
                else
                {
                    answer.CorrectAnswers=+1;
                    db.Entry(answer).State = EntityState.Modified;
                }


                db.Questions.Add(question);

                test.Questions.Add(question);
                profil.Tests.Add(test);



                db.Entry(profil).State = EntityState.Modified;
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();



                List<int> ids = new List<int>();
                foreach (var a in test.Questions.Where(b=>b.Score==1))
                {
                    ids.Add(flashCards.IndexOf(a.flashCard));
                    

                }

                int i = 0;
                do
                {
                    i++;
                    pageNumber = rnd.Next(flashCards.Count);
                   
                }
                while (ids.Contains(pageNumber) && i<100);
            }


            pageNumber++;
            if (know == null)
            {
                return View(flashCards.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                String userid = User.Identity.GetUserName();
                List<Profile> profile = db.Profiles.ToList();
                Profile profil = profile.Find(x => x.UserName == userid);
                List<Test> tests = db.Tests.ToList();
                var test = tests.OrderByDescending(a => a.ID).First();
                if (test.Questions.Count>=20)
                {
                    return RedirectToAction("Stats/"+test.ID);
                }

            }
            return View(flashCards.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Stats(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test= db.Tests.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test.Questions);
        }

        // GET: FlashCards/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlashCard flashCard = db.FlashCards.Find(id);
            if (flashCard == null)
            {
                return HttpNotFound();
            }
            return View(flashCard);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Test(int ?id, int? nr, int? IDCH  )
        {
            int i = 0;
            Random random = new Random((int)DateTime.Now.Ticks+i);
            if (IDCH == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(IDCH);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            if(id!=null && nr!=null)
            {
                id=chapter.FlashCards.IndexOf(db.FlashCards.Find(id));
                nr = chapter.FlashCards.IndexOf(db.FlashCards.Find(nr));
            }
           
            if(nr!=null)
            {
                String userid = User.Identity.GetUserName();
                List<Profile> profile = db.Profiles.ToList();
                Profile profil = profile.Find(x => x.UserName == userid);
                List<Test> tests = db.Tests.ToList();
                var test = tests.OrderByDescending(c => c.ID).First();
                int nrq = test.Questions.Count + 1;
                List<Answer> answers = db.Answers.Where(c => c.UserID == User.Identity.Name).ToList();
                Answer answer = answers.Find(c => c.FlashCard.ID == chapter.FlashCards[id.GetValueOrDefault()].ID);
                Question question = null;
                if (nr==id)
                {
                   
                    question = new Question { ID = nrq, flashCard=chapter.FlashCards[id.GetValueOrDefault()], Score = 1 };
                   if(answer==null)
                    { 
                    if (db.Answers.Count() == 0)
                    {
                        Answer answer1 = new Answer { ID = 0, FlashCard = chapter.FlashCards[id.GetValueOrDefault()], UserID = User.Identity.Name, CorrectAnswers = 1 };
                        db.Answers.Add(answer1);
                    }
                    else
                    {
                        int answerid = db.Answers.OrderByDescending(G => G.ID).Select(G => G.ID).First();
                        Answer answer1 = new Answer { ID = answerid, FlashCard = chapter.FlashCards[id.GetValueOrDefault()], UserID = User.Identity.Name, CorrectAnswers = 1 };
                        db.Answers.Add(answer1);
                    }
                }
                else
                {
                    answer.CorrectAnswers +=1;
                    db.Entry(answer).State = EntityState.Modified;
                }

            }
                else   if (nr != id)
                {
                    
                  question = new Question { ID = nrq, flashCard = chapter.FlashCards[id.GetValueOrDefault()], Score = 0 };
                    if (answer == null)
                    {
                        if (db.Answers.Count() == 0)
                        {
                            Answer answer1 = new Answer { ID = 0, FlashCard = chapter.FlashCards[id.GetValueOrDefault()], UserID = User.Identity.Name, CorrectAnswers = 0 };
                            db.Answers.Add(answer1);
                        }
                        else
                        {
                            int answerid = db.Answers.OrderByDescending(G => G.ID).Select(G => G.ID).First();
                            Answer answer1 = new Answer { ID = answerid, FlashCard = chapter.FlashCards[id.GetValueOrDefault()], UserID = User.Identity.Name, CorrectAnswers = 0 };
                            db.Answers.Add(answer1);
                        }
                    }
                    else
                    {
                        answer.CorrectAnswers = 0;
                        db.Entry(answer).State = EntityState.Modified;
                    }

                        
                }

                db.Questions.Add(question);

                test.Questions.Add(question);
                profil.Tests.Add(test);



                db.Entry(profil).State = EntityState.Modified;
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                if (db.Tests.OrderByDescending(t => t.ID).First().Questions.Count >= 20)
                {

                    return RedirectToAction("Stats/" + test.ID);
                }
            }
            else
            {
                Test test123 = new Test();
                db.Tests.Add(test123);
                db.SaveChanges();

            }
            List<FlashCard> flashCards2 = new List<FlashCard>();

            int a = 0;
            do
            {


                a = rng.Next(chapter.FlashCards.Count());

                


                if (chapter.FlashCards.Count >= 6 && flashCards2.Contains(chapter.FlashCards[a]))
                {

                }
                else
                {
                    flashCards2.Add(chapter.FlashCards[a]);
                }
            } while (flashCards2.Count < 6);
            int b = rng.Next(6);
            String ab = flashCards2[b].ID.ToString();
            ViewData["ID"] = flashCards2[b].ID.ToString();
            ViewData["IDCH"] = chapter.ID.ToString();

            if (chapter.FlashCards.Count < 6)
            {

                return Content("Add flashcard to chapter to continue"); ;

            }
            return View(Shuffle(flashCards2));

        }

        

        public static List<FlashCard> Shuffle( List<FlashCard> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                FlashCard value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
            
        }
        // GET: FlashCards/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FlashCards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Word1,Word2")] FlashCard flashCard)
        {
            if (ModelState.IsValid)
            {
                flashCard.UserID = User.Identity.Name;
                db.FlashCards.Add(flashCard);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(flashCard);
        }
       

        // GET: FlashCards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlashCard flashCard = db.FlashCards.Find(id);
           
            if (flashCard == null)
            {
                return HttpNotFound();
            }
            return View(flashCard);
        }

        // POST: FlashCards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Word1,Word2")] FlashCard flashCard)
        {
            if (ModelState.IsValid)
            {
                db.Entry(flashCard).State = EntityState.Modified;
                db.SaveChanges();
                var id = db.Chapters.Where(c => c.FlashCards.Select(a => a.ID).Contains(flashCard.ID)).Select(a => a.ID).First();
                return RedirectToAction("Details/" + id, "Chapters");
            }
            var id2 = db.Chapters.Where(c => c.FlashCards.Select(a => a.ID).Contains(flashCard.ID)).Select(a => a.ID).First();
            return RedirectToAction("Details/" + id2, "Chapters");
        }

        // GET: FlashCards/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlashCard flashCard = db.FlashCards.Find(id);
            if (flashCard == null)
            {
                return HttpNotFound();
            }
            return View(flashCard);
        }

        // POST: FlashCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        { 

            FlashCard flashCard = db.FlashCards.Find(id);
            
         var id2 = db.Chapters.Where(c=>c.FlashCards.Select(a=>a.ID).Contains(id)).Select(a=>a.ID).First();
           
            db.SaveChanges();
            List<Question> Questions2 = db.Questions.ToList(); ;
            db.Questions.RemoveRange(Questions2);
            Questions2.Clear();
            List<Test> test2 = db.Tests.ToList(); ;
            db.Tests.RemoveRange(test2);
            test2.Clear();
           List<Answer> answers= db.Answers.Where(a=>a.FlashCard.ID==id).ToList();
            db.Answers.RemoveRange(answers);
            db.FlashCards.Remove(flashCard);
            db.SaveChanges();
            return RedirectToAction("Details/"+id2,"Chapters");
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
