using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.ViewModels
{
    public class SettingsViewModel
    {
        public IndexViewModel IndexViewModel { get; set; }
        public SetPasswordViewModel SetPasswordViewModel { get; set; }
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }
    }
}