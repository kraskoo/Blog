namespace Blog.Models.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReplyBindingModel
    {
        [Required]
        public int ReplyId { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string ReplierId { get; set; }

        [Required]
        public int TopicId { get; set; }
        
        public string ReplyDateString { get; set; }
    }
}