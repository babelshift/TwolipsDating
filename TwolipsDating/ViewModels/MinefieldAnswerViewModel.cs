using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwolipsDating.ViewModels
{
    public class MinefieldAnswerViewModel
    {
        public int AnswerId { get; set; }
        public string Content { get; set; }
        public bool IsSelected { get; set; }
        public bool IsCorrect { get; set; }
    }
}
