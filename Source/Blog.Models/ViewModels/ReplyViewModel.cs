namespace Blog.Models.ViewModels
{
    using System;

    public class ReplyViewModel
    {
        public int Id { get; set; }

        public string ReplierId { get; set; }

        public string ReplierProfilePicture { get; set; }

        public string ReplierUserName { get; set; }

        public DateTime ReplyDate { get; set; }

        public string ReplyText { get; set; }
    }
}