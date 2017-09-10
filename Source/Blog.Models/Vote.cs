namespace Blog.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Vote
    {
        public int Id { get; set; }

        [Required]
        [Range(typeof(int), "-1", "1")]
        public int Rate { get; set; }

        [Required]
        public int ReplyId { get; set; }

        public virtual Reply Reply { get; set; }

        public ICollection<User> Voters { get; set; }
    }
}