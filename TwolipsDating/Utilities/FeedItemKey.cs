namespace TwolipsDating.Utilities
{
    public class FeedItemKey
    {
        public string UserId { get; set; }
        public string TimeAgo { get; set; }

        public override bool Equals(object obj)
        {
            var feedItem = obj as FeedItemKey;
            if (feedItem != null)
            {
                if (UserId == feedItem.UserId && TimeAgo == feedItem.TimeAgo)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = UserId.GetHashCode() + TimeAgo.GetHashCode();
            return hash;
        }
    }
}