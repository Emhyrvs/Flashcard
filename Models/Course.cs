using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flashcards.Models
{
    public class Course
    {
        public int ID { get; set; }
        public String Name { get; set; }
       
        public virtual List<Chapter> Chapters { get; set; }
        public String IDUSER { get; set; }
    }
}