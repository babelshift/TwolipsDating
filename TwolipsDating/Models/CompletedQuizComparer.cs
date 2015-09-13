using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Models
{
    public class CompletedQuizComparer : IEqualityComparer<CompletedQuiz>
    {
        public bool Equals(CompletedQuiz cq1, CompletedQuiz cq2)
        {
            if(cq1 == null || cq2 == null)
            {
                return false;
            }

            return (cq1.QuizId == cq2.QuizId);
        }

        public int GetHashCode(CompletedQuiz cq)
        {
            return cq.QuizId.GetHashCode();
        }
    }
}