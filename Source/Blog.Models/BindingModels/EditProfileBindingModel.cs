namespace Blog.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Attributes;

    public class EditProfileBindingModel
    {
        [Required]
        [Display(Name = "Fist Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [MaxFileSize(5242880)]
        [DataType(DataType.Upload)]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase PostedFileBase { get; set; }

        [Display(Name = "Profile Picture Url")]
        public string ProfilePictureUrl { get; set; }
    }
}