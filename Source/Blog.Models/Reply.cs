namespace Blog.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Reply
    {
        public int Id { get; set; }

        [Required]
        public DateTime ReplayDate { get; set; }

        [Required]
        public string ReplayText { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public int TopicId { get; set; }

        public virtual Topic Topic { get; set; }
    }
}