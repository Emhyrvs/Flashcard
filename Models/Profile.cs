using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flashcards.Models
{
    public class Profile
    {
        public int ID { get; set; }
        public String UserName { get; set; }
        public virtual List<Answer> Answers { get; set; }
        public virtual List<Test> Tests { get; set; }
    }
}