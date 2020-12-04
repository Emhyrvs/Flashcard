using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flashcards.Models
{
    public class Answer
    {
        public int ID { get; set; }
        public  virtual FlashCard FlashCard { get; set; }
        public int CorrectAnswers { get; set; }
        public String UserID { get; set; }
    }
}