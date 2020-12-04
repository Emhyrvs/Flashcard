using Flashcards.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Flashcards.DAL
{
    public class FlashcardsInitlializer : DropCreateDatabaseIfModelChanges<FlashcardsContext>
    {
        protected override void Seed(FlashcardsContext context)
        {
            var flashCards = new List<FlashCard>
            {
                new FlashCard{ID=1,Word1="Super",Word2="Nice" },
                 new FlashCard{ID=2,Word1="żenada ",Word2="Cringe" },
                   new FlashCard{ID=3,Word1="Arcydzieło  ",Word2="Masterpice" }

            };


            flashCards.ForEach(m => context.FlashCards.Add(m));
            context.SaveChanges();
            var chapters = new List<Chapter>
            {
                new Chapter{ ID=1,Name="Chapter1",FlashCards=new List<FlashCard> {flashCards[0],flashCards[1]}},
                new Chapter{ID=2,Name="Chapter2",FlashCards=new List<FlashCard> { flashCards[2]} }

            };
            chapters.ForEach(m => context.Chapters.Add(m));
            context.SaveChanges();
            var courses = new List<Course>
            {
                new Course{ ID=1,Name="English-Polish Course",Chapters=new List<Chapter> {chapters[1]}},
                new Course{ID=2,Name="History Course"}

            };
            courses.ForEach(m => context.Courses.Add(m));
            context.SaveChanges();
            
            context.SaveChanges();
            var profiles = new List<Profile>
            {
                new Profile{ID=1,UserName="maciejzakrzewski1998@gmail.com"  },
                new Profile{ID=2,UserName="Admin@gmail.com"}

            };
            profiles.ForEach(m => context.Profiles.Add(m));
            context.SaveChanges();
            var questions = new List<Question>
            {
                
            };
            questions.ForEach(m => context.Questions.Add(m));
            context.SaveChanges();

            var test = new List<Test>
            {
                            };
            test.ForEach(m => context.Tests.Add(m));
            context.SaveChanges();



            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            roleManager.Create(new IdentityRole("Admin"));
            var user = new ApplicationUser { UserName = "Admin@gmail.com" };
            String password = "Admin1";
            userManager.Create(user, password);
            userManager.AddToRole(user.Id, "Admin");


        }
    }
}
