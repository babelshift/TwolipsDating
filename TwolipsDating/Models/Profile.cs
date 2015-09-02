using System;
using System.Collections.Generic;

namespace TwolipsDating.Models
{
    public class Profile
    {
        public Profile()
        {
            Tags = new List<Tag>();
            TagSuggestions = new List<TagSuggestion>();
            FavoritedBy = new List<FavoriteProfile>();
            TagAwards = new List<TagAward>();
        }

        public int Id { get; set; }
        public DateTime Birthday { get; set; }
        public int GenderId { get; set; }
        public int? GeoCityId { get; set; }
        public int? UserImageId { get; set; }
        public int? SelectedTitleId { get; set; }
        public string SummaryOfSelf { get; set; }
        public string SummaryOfDoing { get; set; }
        public string SummaryOfGoing { get; set; }
        public int? LookingForAgeMin { get; set;}
        public int? LookingForAgeMax { get; set;}
        public int? LookingForTypeId { get; set; }
        public int? RelationshipStatusId { get; set; }
        public int? LookingForLocationId { get; set; }

        public virtual UserImage UserImage { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual GeoCity GeoCity { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual StoreItem SelectedTitle { get; set; }
        public virtual LookingForType LookingForType { get; set; }
        public virtual RelationshipStatus RelationshipStatus { get; set; }
        public virtual LookingForLocation LookingForLocation { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<TagSuggestion> TagSuggestions { get; set; }
        public virtual ICollection<FavoriteProfile> FavoritedBy { get; set; }
        public virtual ICollection<TagAward> TagAwards { get; set; }
        public virtual ICollection<ProfileViewLog> VisitedBy { get; set; }
        public virtual ICollection<Language> Languages { get; set; }
    }
}