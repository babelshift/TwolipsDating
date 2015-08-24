namespace TwolipsDating.Utilities
{
    public class TagSuggestionFeedItemKey
    {
        public string UserId { get; set; }
        public string TimeAgo { get; set; }
        public int ProfileId { get; set; }

        public override bool Equals(object obj)
        {
            var feedItem = obj as TagSuggestionFeedItemKey;
            if (feedItem != null)
            {
                if (UserId == feedItem.UserId 
                    && TimeAgo == feedItem.TimeAgo
                    && ProfileId == feedItem.ProfileId)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = UserId.GetHashCode() + TimeAgo.GetHashCode() + ProfileId.GetHashCode();
            return hash;
        }
    }
}