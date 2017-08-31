using Blog.Models.Enums;

namespace Blog.WebApplication.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Controllers;
    using Common.Extensions;
    using Models;

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
            IEnumerable<Topic> topics)
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
    }
}