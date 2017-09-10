namespace Blog.Models.ViewModels.AccountViewModels
{
    using System.Collections.Generic;
    using Microsoft.Owin.Security;

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }

        public IEnumerable<AuthenticationDescription> AuthenticationDescriptions { get; set; }
    }
}