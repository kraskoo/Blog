namespace Blog.Services.AccountServices
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using Models;
    using Models.BindingModels;

    public class AccountProfileService
    {
        private const string NoProfileImagePath = "/Users/no-profile-image.png";
        private readonly ConcurrentDictionary<string, string> base64StringsByUserProfileImage;

        public AccountProfileService()
        {
            this.base64StringsByUserProfileImage = new ConcurrentDictionary<string, string>();
        }

        public async Task UploadUserProfilePicture(
            string username,
            bool hasOwnProfilePicture,
            EditProfileBindingModel epbm)
        {
            var memoryStream = new MemoryStream();
            epbm.PostedFileBase.InputStream.CopyTo(memoryStream);
            var imageBytes = InternalService.ImageService
                .Load(memoryStream.ToArray())
                .GetRectangledImage()
                .GetManualSizedRectangle(250)
                .To72DPI()
                .ToJpeg()
                .ToByteArray();
            var userPath = this.ConvertUsernameToApiDirectory(username);
            if (!InternalService.DropboxService.IsDirectoryExists(userPath))
            {
                InternalService.DropboxService.CreateDirectory(userPath);
            }

            var filePath = $"{userPath}/profile.jpg";
            await InternalService.DropboxService.UploadFile(filePath, imageBytes, hasOwnProfilePicture);
        }

        public string GetUserProfileImage(bool hasOwnProfileImage, string username, bool needsToReload = false)
        {
            var userProfileImage = $"{this.ConvertUsernameToApiDirectory(username)}/profile.jpg";
            if (needsToReload)
            {
                base64StringsByUserProfileImage.TryRemove(userProfileImage, out string _);
            }

            if (!base64StringsByUserProfileImage.ContainsKey(hasOwnProfileImage ? userProfileImage : NoProfileImagePath))
            {
                base64StringsByUserProfileImage.TryAdd(
                    userProfileImage,
                    this.ConvertToBase64String(
                        InternalService.DropboxService
                            .GetRetBytesFromApiUrl(
                                !hasOwnProfileImage ?
                                NoProfileImagePath :
                                userProfileImage)));
            }

            return base64StringsByUserProfileImage[userProfileImage];
        }

        public string GetUserProfileImage(User user, bool needsToReload = false)
        {
            var userProfileImage = $"{this.ConvertUsernameToApiDirectoryByUser(user)}/profile.jpg";
            if (needsToReload)
            {
                base64StringsByUserProfileImage.TryRemove(userProfileImage, out string _);
            }

            if (!base64StringsByUserProfileImage.ContainsKey(user.HasOwnProfilePicture ? userProfileImage : NoProfileImagePath))
            {
                base64StringsByUserProfileImage.TryAdd(
                    userProfileImage,
                    this.ConvertToBase64String(
                        InternalService.DropboxService
                            .GetRetBytesFromApiUrl(
                                !user.HasOwnProfilePicture ?
                                NoProfileImagePath :
                                userProfileImage)));
            }

            return base64StringsByUserProfileImage[userProfileImage];
        }

        private string ConvertUsernameToApiDirectoryByUser(User user)
        {
            return $"/Users/{user.UserName.ToLower().Replace(" ", "-")}";
        }

        private string ConvertUsernameToApiDirectory(string username)
        {
            return $"/Users/{username.ToLower().Replace(" ", "-")}";
        }

        public string ConvertToBase64String(byte[] bytes)
        {
            return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
        }
    }
}