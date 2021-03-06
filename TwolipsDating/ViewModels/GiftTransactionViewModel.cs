﻿namespace TwolipsDating.ViewModels
{
    public class GiftTransactionViewModel
    {
        public int SenderProfileId { get; set; }
        public string SenderProfileImagePath { get; set; }
        public string SenderUserName { get; set; }
        public string GiftImagePath { get; set; }
        public int GiftAmount { get; set; }
        public int GiftTransactionId { get; set; }
        public string DateTransaction { get; set; }
    }
}