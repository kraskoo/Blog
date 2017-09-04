namespace Blog.WebApplication
{
    using System;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.Google;
    using Owin;
    using Models;
    using Data;
    using Services;

    public partial class Startup
    {
        private const string FacebookName = "Facebook";
        private const string GoogleName = "Google";
        private static string FacebookId;
        private static string FacebookSecret;
        private static string GoogleKey;
        private static string GoogleSecrete;

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            SetupFacebookInfo();
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(BlogDbContext.Create);
            app.CreatePerOwinContext<UserManager>(UserManager.Create);
            app.CreatePerOwinContext<UserSignInManager>(UserSignInManager.Create);
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator
                        .OnValidateIdentity<UserManager, User>(
                            TimeSpan.FromMinutes(30),
                            (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            app.UseFacebookAuthentication(FacebookId, FacebookSecret);
            app.UseGoogleAuthentication(GetNewGoogleAuthenticationOptions());
        }

        private static GoogleOAuth2AuthenticationOptions GetNewGoogleAuthenticationOptions()
        {
            if (string.IsNullOrEmpty(GoogleKey))
            {
                var googleApi = BlogDbContext.Create()
                    .ApiConnections
                    .FirstOrDefault(api => api.ApiName == GoogleName);
                GoogleKey = googleApi.Key;
                GoogleSecrete = googleApi.Secrete;
            }

            return new GoogleOAuth2AuthenticationOptions
            {
                ClientId = GoogleKey,
                ClientSecret = GoogleSecrete
            };
        }

        private static void SetupFacebookInfo()
        {
            if (string.IsNullOrEmpty(FacebookId))
            {
                var facebookApi = BlogDbContext.Create()
                    .ApiConnections
                    .FirstOrDefault(api => api.ApiName == FacebookName);
                FacebookId = facebookApi.Key;
                FacebookSecret = facebookApi.Secrete;
            }
        }
    }
}