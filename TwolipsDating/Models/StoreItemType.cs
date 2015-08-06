using System.Collections.Generic;

namespace TwolipsDating.Models
{
    public enum StoreItemTypeValues
    {
        Gift = 1,
        Title = 2
    }

    public class StoreItemType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<StoreItem> StoreItems { get; set; }
    }
}