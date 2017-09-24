namespace Blog.Models.ViewModels
{
    using System;
    using Enums;

    public class TopicViewModel2
    {
        public string AuthorProfilePicture { get; set; }

        public string AuthorUserName { get; set; }

        public int TopicId { get; set; }

        public string TopicTitle { get; set; }

        public DateTime TopicDate { get; set; }

        public TopicCategory TopicCategory { get; set; }

        public string TopicText { get; set; }

        //public IEnumerable<ReplyViewModel> ReplyViewModels { get; set; }
    }
}