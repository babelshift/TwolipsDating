using System;
using System.Collections.Generic;
using System.Linq;
using TwolipsDating.Models;

namespace TwolipsDating.ViewModels
{
    public class QuizViewModel
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public int QuizTypeId { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
        public MinefieldQuestionViewModel MinefieldQuestion { get; set; }
        public IReadOnlyCollection<TagViewModel> Tags { get; set; }
        public bool IsAlreadyCompleted { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string ImageUrl { get; set; }
        public string SEOName { get; set; }

        public int AverageQuestionPoints
        {
            get
            {
                if (QuizTypeId == (int)QuizTypeValues.Individual)
                {
                    if (Questions != null && Questions.Count > 0)
                    {
                        return (int)Math.Round(Questions.Average(x => x.Points));
                    }
                }
                else
                {
                    if (MinefieldQuestion != null)
                    {
                        return MinefieldQuestion.Points;
                    }
                }

                return 0;
            }
        }

        public int PointsPossible
        {
            get
            {
                if (QuizTypeId == (int)QuizTypeValues.Individual)
                {
                    if (Questions != null && Questions.Count > 0)
                    {
                        return Questions.Sum(x => x.Points);
                    }
                }
                else
                {
                    if (MinefieldQuestion != null)
                    {
                        return MinefieldQuestion.Points * PossibleCorrectAnswerCount;
                    }
                }

                return 0;
            }
        }

        public int UserScorePercent
        {
            get
            {
                if (QuizTypeId == (int)QuizTypeValues.Individual)
                {
                    return (int)Math.Round(((double)CorrectAnswerCount / (double)Questions.Count) * 100);
                }
                else
                {
                    if(MinefieldQuestion != null)
                    {
                        return (int)Math.Round(((double)CorrectAnswerCount / (double)PossibleCorrectAnswerCount * 100));
                    }    
                }

                return 0;
            }
        }

        public int PossibleCorrectAnswerCount
        {
            get
            {
                if (QuizTypeId == (int)QuizTypeValues.Individual)
                {
                    return Questions.Count;
                }
                else
                {
                    if(MinefieldQuestion != null)
                    {
                        return MinefieldQuestion.Answers.Count(x => x.IsCorrect);
                    }
                }

                return 0;
            }
        }

        public int CorrectAnswerCount
        {
            get
            {
                if (QuizTypeId == (int)QuizTypeValues.Individual)
                {
                    return Questions
                        .Where(x => x.CorrectAnswerId == x.SelectedAnswerId)
                        .Count();
                }
                else
                {
                    // n^2?
                    int correctAnswerCount = 0;

                    if (MinefieldQuestion != null)
                    {
                        foreach (var selectedAnswer in MinefieldQuestion.Answers)
                        {
                            if (selectedAnswer.IsSelected && selectedAnswer.IsCorrect)
                            {
                                correctAnswerCount++;
                            }
                        }
                    }

                    return correctAnswerCount;
                }
            }
        }

        public int PointsEarned
        {
            get
            {
                if (QuizTypeId == (int)QuizTypeValues.Individual)
                {
                    return Questions
                        .Where(x => x.CorrectAnswerId == x.SelectedAnswerId)
                        .Sum(x => x.Points);
                }
                else
                {
                    int sumPoints = 0;

                    if (MinefieldQuestion != null)
                    {
                        sumPoints = MinefieldQuestion.Points * CorrectAnswerCount;
                    }

                    return sumPoints;
                }
            }
        }

        public IReadOnlyCollection<UserCompletedQuizViewModel> UsersCompletedQuiz { get; set; }
        public IReadOnlyCollection<QuizOverviewViewModel> SimilarQuizzes { get; set; }
        public IReadOnlyCollection<QuizCategoryViewModel> QuizCategories { get; set; }
        public IReadOnlyCollection<UserWithSimilarQuizScoreViewModel> UsersWithSimilarScores { get; set; }

        public QuestionViolationViewModel QuestionViolation { get; set; }

        public AchievementProgressViewModel PointsObtainedProgress { get; set; }
        public AchievementProgressViewModel QuizzesCompletedProgress { get; set; }
        public AchievementProgressViewModel TagsAwardedProgress { get; set; }
        public IReadOnlyCollection<AchievementUnlockedViewModel> UnlockedAchievements { get; set; }
    }
}