namespace Blog.Services.DataServices
{
    using System;
    using System.Data.Entity;
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
        public async Task EditProfile(
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
                    var accountUserProfile = new AccountProfileService();
                    await accountUserProfile.UploadUserProfilePicture(
                            currentUser.UserName,
                            currentUser.HasOwnProfilePicture,
                            epbm);
                    if (!currentUser.HasOwnProfilePicture)
                    {
                        currentUser.HasOwnProfilePicture = true;
                        context.Users.AddOrUpdate(currentUser);
                        await context.SaveChangesAsync();
                    }
                }

                if (isUserInfoIsUpdated)
                {
                    currentUser.FirstName = epbm.FirstName;
                    currentUser.LastName = epbm.LastName;
                    currentUser.Email = epbm.Email;
                    var newUsername = $"{epbm.FirstName} {epbm.LastName}";
                    if (currentUser.HasOwnProfilePicture)
                    {
                        InternalService.DropboxService
                            .RenameFolder("/Users/",
                                currentUser.UserName.Replace(" ", "-").ToLower(),
                                newUsername.Replace(" ", "-").ToLower());
                    }

                    currentUser.UserName = newUsername;
                    context.Users.AddOrUpdate(currentUser);
                    await context.SaveChangesAsync();
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

        public IEnumerable<UserReplyViewModel> GetUserReplies(string id)
        {
            User currentUser;
            string imageUrl;
            Reply[] replies;
            using (var context = this.GetDbContext)
            {
                currentUser = context.Users.Find(id);
                imageUrl = new AccountProfileService().GetUserProfileImage(currentUser);
                replies = context
                    .Replies
                    .Include(r => r.Topic)
                    .Where(r => r.UserId == id)
                    .ToArray();
            }
            
            return replies.Select(
                r => new UserReplyViewModel
                {
                    ReplyId = r.Id,
                    ReplyText = r.ReplayText,
                    ReplayDate = r.ReplayDate,
                    TopicId = r.TopicId,
                    TopicTitle = r.Topic.Title,
                    UserId = currentUser.Id,
                    Username = currentUser.UserName,
                    UserProfileImage = imageUrl
                });
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

        public EditProfileBindingModel GetEditProfile(
            string userId,
            Func<bool, string, bool, string> profileImageFunc)
        {
            var currentUser = this.FindUserById(userId);
            return new EditProfileBindingModel
            {
                Email = currentUser.Email,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                ProfilePictureUrl = profileImageFunc(
                    currentUser.HasOwnProfilePicture,
                    currentUser.UserName,
                    true)
            };
        }
    }
}