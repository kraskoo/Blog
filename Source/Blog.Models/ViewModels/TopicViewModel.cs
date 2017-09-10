namespace Blog.Models.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Enums;

    public class TopicViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        [Display(Name = "Topic Date")]
        public DateTime TopicDate { get; set; }

        public TopicCategory Category { get; set; }

        [Display(Name = "Comment Count")]
        public int CommentCount { get; set; }

        [Display(Name = "Users Activity")]
        public int UsersActivityCount { get; set; }

        [Display(Name = "Votes Rate")]
        public double VotesRate { get; set; }

        [Display(Name = "Last Activity")]
        public DateTime? LastActivity { get; set; }
    }
}