namespace Blog.Services.DataServices
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Security.Principal;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Security;
    using AccountServices;
    using Models.BindingModels;
    using Models;
    using Models.ViewModels.AccountViewModels;
    using System.Collections.Generic;

    public class AccountDataService : BaseDataService
    {
        public void EditProfile(
            EditProfileBindingModel epbm,
            IPrincipal user,
            IAuthenticationManager authenticationManager,
            UserManager userManager)
        {
            var isImageUpdate = epbm.PostedFileBase != null;
            using (var context = this.GetDbContext)
            {
                var currentUser = context.Users.Find(user.Identity.GetUserId());
                var isUserInfoIsUpdated = epbm.Email != currentUser.Email ||
                                          epbm.FirstName != currentUser.FirstName ||
                                          epbm.LastName != currentUser.LastName;
                if (isImageUpdate)
                {
                    new AccountProfileService()
                        .UploadUserProfilePicture(
                            currentUser.UserName,
                            currentUser.HasOwnProfilePicture,
                            epbm);
                    if (!currentUser.HasOwnProfilePicture)
                    {
                        currentUser.HasOwnProfilePicture = true;
                        context.Users.AddOrUpdate(currentUser);
                        context.SaveChanges();
                    }
                }

                if (isUserInfoIsUpdated)
                {
                    currentUser.FirstName = epbm.FirstName;
                    currentUser.LastName = epbm.LastName;
                    currentUser.Email = epbm.Email;
                    var newUsername = $"{epbm.FirstName} {epbm.LastName}";
                    InternalService.DropboxService
                        .RenameFolder("/Users/",
                            currentUser.UserName.Replace(" ", "-").ToLower(),
                            newUsername.Replace(" ", "-").ToLower());
                    currentUser.UserName = newUsername;
                    context.Users.AddOrUpdate(currentUser);
                    context.SaveChanges();
                    authenticationManager.SignOut(
                        DefaultAuthenticationTypes.ExternalCookie);
                    var identity = userManager.CreateIdentity(
                        currentUser,
                        DefaultAuthenticationTypes.ApplicationCookie);
                    authenticationManager.SignIn(
                        new AuthenticationProperties
                        {
                            IsPersistent = true
                        },
                        identity);
                }
            }
        }

        public IEnumerable<Reply> GetUserComments(string id)
        {
            return this.GetDbContext.Replies.Where(r => r.UserId == id).ToList();
        }

        public void SetUserRole(string userId, string roleId)
        {
            using (var context = this.GetDbContext)
            {
                context.Roles.Find(roleId).Users
                    .Add(new IdentityUserRole
                    {
                        UserId = userId,
                        RoleId = roleId
                    });
                context.SaveChanges();
            }
        }

        public User FindUserById(string id)
        {
            return this.GetDbContext.Users.Find(id);
        }

        public string GetResolvedUserRole(RoleManager roleManager)
        {
            var adminRole = roleManager.FindByName("Admin");
            var regularRole = roleManager.FindByName("Regular");
            return !adminRole.Users.Any() ? adminRole.Id : regularRole.Id;
        }

        public string HashPassword(string password)
        {
            return new PasswordHasher().HashPassword(password);
        }

        public Task<IdentityResult> CreateNewApplicationUser(
            RegisterViewModel model,
            RoleManager roleManager,
            UserManager userManager,
            out User user,
            out string roleId)
        {
            user = new User
            {
                PasswordHash = this.HashPassword(model.Password),
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = $"{model.FirstName} {model.LastName}",
                Email = model.Email
            };

            roleId = this.GetResolvedUserRole(roleManager);
            return userManager.CreateAsync(user, model.Password);
        }
    }
}