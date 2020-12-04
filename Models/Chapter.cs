using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flashcards.Models
{
    public class Chapter
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public virtual List<FlashCard> FlashCards { get; set; }
        public String UserID { get; set; }
      
    }
}