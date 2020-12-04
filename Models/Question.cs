using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flashcards.Models
{
    public class Question
    {
        public int ID { get; set; }
        public virtual FlashCard flashCard { get; set; }
        public int Score { get; set; }
    }
}