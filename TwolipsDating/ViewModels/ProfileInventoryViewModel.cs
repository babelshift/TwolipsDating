using System.Collections.Generic;

namespace TwolipsDating.ViewModels
{
    public class ProfileInventoryViewModel
    {
        public IReadOnlyCollection<InventoryItemViewModel> Items { get; set; }
        public string CurrentUserId { get; set; }
        public string ProfileUserId { get; set; }
    }
}