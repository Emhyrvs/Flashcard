using Flashcards.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Flashcards.DAL
{
    public class FlashcardsContext:DbContext
    {
        public FlashcardsContext() : base("Default Connection")
        {

        }
        public DbSet<FlashCard> FlashCards { get; set; }

        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet <Answer> Answers { get; set; }


    }
}