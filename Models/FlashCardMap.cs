using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flashcards.Models
{
    sealed class FlashCardMap : CsvMap
        public FlashCardMap()
        {
            Map(m => m.Word1).Index(0);
            Map(m => m.Word2).Index(1);
         
        }
    }

    
}