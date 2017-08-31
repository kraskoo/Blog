namespace Blog.Models.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum TopicCategory
    {
        [Display(Name = "Asp Net")]
        AspNet,
        [Display(Name = "Entity Framework")]
        EntityFramework,
        Html,
        Css,
        JavaScript,
        [Display(Name = "Design Patterns")]
        DesignPatterns
    }
}