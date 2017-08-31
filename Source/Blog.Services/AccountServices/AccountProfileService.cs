namespace Blog.Services.AccountServices
{
    using System.IO;
    using Models;
    using Models.BindingModels;

    public class AccountProfileService
    {
        private const string NoProfileImagePath = "/Users/no-profile-image.png";

        public void UploadUserProfilePicture(
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
            InternalService.DropboxService.UploadFile(filePath, imageBytes, hasOwnProfilePicture);
        }

        public string GetUserProfileImage(bool hasOwnProfileImage, string username)
        {
            return !hasOwnProfileImage ?
                InternalService.DropboxService.GetUrlToApiFile(NoProfileImagePath) :
                InternalService
                    .DropboxService
                    .GetUrlToApiFile(
                        $"{this.ConvertUsernameToApiDirectory(username)}/profile.jpg");
        }

        public string GetUserProfileImage(User user)
        {
            return !user.HasOwnProfilePicture ?
                InternalService.DropboxService.GetUrlToApiFile(NoProfileImagePath) :
                InternalService
                    .DropboxService
                    .GetUrlToApiFile(
                        $"{this.ConvertUsernameToApiDirectoryByUser(user)}/profile.jpg");
        }

        private string ConvertUsernameToApiDirectoryByUser(User user)
        {
            return $"/Users/{user.UserName.ToLower().Replace(" ", "-")}";
        }

        private string ConvertUsernameToApiDirectory(string username)
        {
            return $"/Users/{username.ToLower().Replace(" ", "-")}";
        }
    }
}