namespace Blog.Models.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Enums;

    public class TopicBindingModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public TopicCategory Category { get; set; }
        
        [Required]
        public string Text { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Topic Date")]
        public DateTime? TopicDate { get; set; }
    }
}