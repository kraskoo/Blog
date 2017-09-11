namespace Blog.Services.DataServices
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Security.Principal;
    using Microsoft.AspNet.Identity;
    using AccountServices;
    using Models;
    using Models.BindingModels;
    using Models.ViewModels;

    public class TopicDataService : BaseDataService
    {
        public async Task<TopicViewModel2> GetById(int id)
        {
            var profileService = new AccountProfileService();
            Topic currentTopic;
            Reply[] replies;
            using (var context = this.GetDbContext)
            {
                currentTopic = await context
                    .Topics
                    .Include(t => t.User)
                    .Include(t => t.Replies)
                    .FirstOrDefaultAsync(t => t.Id == id);
                replies = await context
                    .Replies
                    .Include(r => r.User)
                    .Include(r => r.Topic)
                    .Where(r => r.TopicId == currentTopic.Id)
                    .ToArrayAsync();
                return new TopicViewModel2
                {
                    TopicId = currentTopic.Id,
                    TopicTitle = currentTopic.Title,
                    TopicDate = currentTopic.TopicDate,
                    TopicCategory = currentTopic.Category,
                    TopicText = currentTopic.Text,
                    AuthorUserName = currentTopic.User.UserName,
                    AuthorProfilePicture = profileService.GetUserProfileImage(currentTopic.User),
                    ReplyViewModels = currentTopic.Replies.AsParallel().Select(r =>
                        new ReplyViewModel
                        {
                            Id = r.Id,
                            ReplyText = r.ReplayText,
                            ReplierId = replies.FirstOrDefault(rep => rep.Id == r.Id).UserId,
                            ReplierProfilePicture =
                                profileService.GetUserProfileImage(replies.FirstOrDefault(rep => rep.Id == r.Id)?.User),
                            ReplierUserName = replies.FirstOrDefault(rep => rep.Id == r.Id).User.UserName,
                            ReplyDate = r.ReplayDate
                        }
                    ).OrderBy(r => r.Id)
                };
            }
        }

        public TopicViewModel[] GetIndexTopics(string category)
        {
            using (var context = this.GetDbContext)
            {
                return !string.IsNullOrEmpty(category) ?
                    context.Topics
                        .Include(t => t.Replies)
                        .Where(t => t.Category.ToString() == category)
                        .Select(t =>
                            new TopicViewModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Text = t.Text,
                                TopicDate = t.TopicDate,
                                Category = t.Category,
                                CommentCount = t.Replies.Count,
                                UsersActivityCount = t.Replies.Select(r => r.User).Distinct().Count(),
                                VotesRate = t.User.Votes.Count == 0 ? 0 : t.User.Votes.Select(v => v.Rate).Sum() / (double)t.User.Votes.Count,
                                LastActivity = t.Replies.OrderBy(r => r.ReplayDate).FirstOrDefault().ReplayDate
                            }).ToArray() :
                    context.Topics
                        .Include(t => t.Replies)
                        .Select(t => new TopicViewModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Text = t.Text,
                                TopicDate = t.TopicDate,
                                Category = t.Category,
                                CommentCount = t.Replies.Count,
                                UsersActivityCount = t.Replies.Select(r => r.User).Distinct().Count(),
                                VotesRate = t.User.Votes.Count == 0 ? 0 : t.User.Votes.Select(v => v.Rate).Sum() / (double)t.User.Votes.Count,
                                LastActivity = t.Replies.OrderBy(r => r.ReplayDate).FirstOrDefault().ReplayDate
                        }).ToArray();
            }
        }

        public void CreateNewTopic(TopicBindingModel topic, IPrincipal user)
        {
            using (var context = this.GetDbContext)
            {
                context.Topics.Add(new Topic
                {
                    Category = topic.Category,
                    UserId = user.Identity.GetUserId(),
                    TopicDate = DateTime.UtcNow,
                    Text = topic.Text,
                    Title = topic.Title
                });
                context.SaveChanges();
            }
        }
    }
}