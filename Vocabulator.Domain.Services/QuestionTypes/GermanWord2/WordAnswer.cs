using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vocabulator.Domain.Services.QuestionTypes.GermanWord;

namespace Vocabulator.Domain.Services.QuestionTypes.GermanWord2
{
    public class WordAnswer
    {
        public string Word { get; set; }
        public TranslationEntry[] Translations { get; set; }
    }
}
