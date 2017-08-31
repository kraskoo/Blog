[assembly: Microsoft.Owin.OwinStartup(typeof(Blog.WebApplication.Startup))]
namespace Blog.WebApplication
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}