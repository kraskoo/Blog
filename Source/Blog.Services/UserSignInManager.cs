namespace Blog.Services
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using Models;

    // Configure the application sign-in manager which is used in this application.
    public class UserSignInManager : SignInManager<User, string>
    {
        public UserSignInManager(
            UserManager userManager,
            IAuthenticationManager authenticationManager) : base(
            userManager,
            authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((UserManager)UserManager);
        }

        public static UserSignInManager Create(
            IdentityFactoryOptions<UserSignInManager> options,
            IOwinContext context)
        {
            return new UserSignInManager(
                context.GetUserManager<UserManager>(),
                context.Authentication);
        }
    }
}