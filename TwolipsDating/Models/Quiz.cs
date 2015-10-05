using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public enum QuizValues
    {
        TechnicalGuru = 1,
        GamerGod,
        CreativeGenius,
        BusinessSense,
        LanguageLinguist,
        WorldOfWarcraft_Scout,
        WorldOfWarcraft_Champion,
        WorldOfWarcraft_HighWarlord,
        MoviesAndMonsters,
        LegendOfZelda,
        MovieQuotes,
        HarryPotter,
        SpacballsTheQuiz,
        TheGreatFoodTruckQuiz,
        TheWalkingDead,
        StarWarsCharacters,
        MoviesOfThe90s,
        OceanDepths,
        SummerOlympics,
        ExoticAnimals,
        StarTrek_TOS,
        FuturamaCharacters,
        WinterOlympics,
        WorldWarII_ThePacific,
    }

    public class Quiz
    {
        public int Id { get; set; }

        [IndexAttribute("UX_Name", 1, IsUnique = true)]
        public string Name { get; set; }

        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int Points { get; set; }
        public bool IsActive { get; set; }
        public string ImageFileName { get; set; }
        public int QuizCategoryId { get; set; }
        public int QuizTypeId { get; set; }

        public virtual QuizType QuizType { get; set; }
        public virtual QuizCategory QuizCategory { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<CompletedQuiz> CompletedByUsers { get; set; }
        public virtual MinefieldQuestion MinefieldQuestion { get; set; } // only populated when QuizType = Minefield
    }
}