namespace Blog.WebApplication.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Controllers;
    using Common.Extensions;
    using Models.Enums;
    using Models.ViewModels;

    public static class HttpExtensions
    {
        public static string GetProfilePictureUrl(this HttpContextBase httpContext, string userId)
        {   
            return AccountController.AccountProfileService
                .GetUserProfileImage(
                    AccountController.AccountDataService
                    .FindUserById(userId));
        }

        public static string GetEnumToString<T>(
            this HttpContextBase httpContext,
            T topicCategory) where T : IComparable, IFormattable, IConvertible
        {
            return topicCategory.GetSplittedPascalCaseEnumTypeName();
        }

        public static string GetTopicCategories(
            this HttpContextBase httpContext,
            IEnumerable<TopicViewModel> topics)
        {
            var topicsCount = new HashSet<TopicCategory>();
            foreach (var topic in topics)
            {
                topicsCount.Add(topic.Category);
            }

            return string.Join(", ", topicsCount
                                        .OrderBy(t => t)
                                        .Select(httpContext.GetEnumToString));
        }

        public static string GetLastActivityAsString(
            this HttpContextBase httpContext,
            DateTime? lastActivity)
        {
            if (lastActivity == null)
            {
                return "No activity for current topic";
            }

            if (DateTime.Now.Subtract(lastActivity.Value) < TimeSpan.FromHours(1))
            {
                return "Last active: Few minutes ago ...";
            }

            var subtraction = DateTime.Now.Subtract(lastActivity.Value);
            if (subtraction < TimeSpan.FromHours(6))
            {
                return $"Last active: About {subtraction.Hours} ago";
            }

            return $"Last active: {lastActivity.Value:d}";
        }
    }
}