namespace Blog.Models.ViewModels.AccountViewModels
{
    using System;

    public class UserReplyViewModel
    {
        public int ReplyId { get; set; }

        public string ReplyText { get; set; }

        public DateTime ReplayDate { get; set; }

        public string UserProfileImage { get; set; }

        public int TopicId { get; set; }

        public string TopicTitle { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }
    }
}