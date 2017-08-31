namespace Blog.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using Enums;

    public class Topic
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public TopicCategory Category { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Topic Date")]
        public DateTime TopicDate { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Reply> Replies { get; set; }
    }
}