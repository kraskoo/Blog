namespace Blog.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ApiConnection
    {
        public int Id { get; set; }

        [Required]
        public string ApiName { get; set; }

        public string Key { get; set; }

        public string Secrete { get; set; }

        public string Token { get; set; }
    }
}