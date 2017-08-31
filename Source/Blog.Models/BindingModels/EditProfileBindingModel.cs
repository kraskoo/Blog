namespace Blog.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;

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
        
        [DataType(DataType.Upload)]
        [Display(Name = "Upload File")]
        public HttpPostedFileBase PostedFileBase { get; set; }

        [Display(Name = "Profile Picture Url")]
        public string ProfilePictureUrl { get; set; }
    }
}