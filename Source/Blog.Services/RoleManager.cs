namespace Blog.Services
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Data;

    public class RoleManager : RoleManager<IdentityRole>
    {
        public RoleManager(IRoleStore<IdentityRole, string> store) : base(store)
        {
        }

        public static RoleManager Create(
            IdentityFactoryOptions<RoleManager> options,
            IOwinContext context)
        {
            return new RoleManager(new RoleStore<IdentityRole>(context.Get<BlogDbContext>()));
        }
    }
}