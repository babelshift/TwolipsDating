using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwolipsDating.ViewModels
{
    public class StoreTitleViewModel
    {
        public int TitleId { get; set; }
        public int PointPrice { get; set; }
        public string TitleName { get; set; }
        public string TitleDescription { get; set; }
        public bool IsAlreadyOwnedByUser { get; set; }
    }
}
