namespace Blog.WebApplication.Controllers
{
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using Services;

    public abstract class ManageAccountBaseController : Controller
    {
        protected const string XsrfKey = "XsrfId";
        private UserSignInManager signInManager;
        private UserManager userManager;

        protected ManageAccountBaseController()
        {
        }

        protected ManageAccountBaseController(
            UserManager userManager,
            UserSignInManager signInManager,
            RoleManager roleManager)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }

        protected IAuthenticationManager AuthenticationManager =>
            this.OwinContext
                .Authentication;

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("Index", "Home");
        }

        public IOwinContext OwinContext => this.HttpContext.GetOwinContext();

        public UserSignInManager SignInManager
        {
            get => this.signInManager ?? HttpContext
                .GetOwinContext()
                .Get<UserSignInManager>();
            private set => this.signInManager = value;
        }

        public UserManager UserManager
        {
            get => this.userManager ?? HttpContext
                .GetOwinContext()
                .GetUserManager<UserManager>();
            private set => this.userManager = value;
        }

        public RoleManager RoleManager => RoleManager.Create(
            this.OwinContext
                .Get<IdentityFactoryOptions<RoleManager>>(),
            this.OwinContext);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (userManager != null)
                {
                    userManager.Dispose();
                    userManager = null;
                }

                if (signInManager != null)
                {
                    signInManager.Dispose();
                    signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        protected class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(
                string provider,
                string redirectUri) : this(
                    provider,
                    redirectUri,
                    null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                this.LoginProvider = provider;
                this.RedirectUri = redirectUri;
                this.UserId = userId;
            }

            public string LoginProvider { get; set; }

            public string RedirectUri { get; set; }

            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        protected bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return user?.PasswordHash != null;
        }

        protected bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return user?.PhoneNumber != null;
        }
    }
}