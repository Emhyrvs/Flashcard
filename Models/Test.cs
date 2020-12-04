using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flashcards.Models
{
    public class Test
    {
        public int ID { get; set; }
        public  virtual List<Question> Questions { get; set; }
    }
}