namespace Blog.Services.DataServices
{
    using System;
    using System.Linq;
    using System.Security.Principal;
    using Microsoft.AspNet.Identity;
    using Models;
    using Models.BindingModels;

    public class TopicDataService : BaseDataService
    {
        public Topic GetById(int id)
        {
            return this.GetDbContext.Topics.Find(id);
        }

        public Topic[] GetIndexTopics(string category)
        {
            using (var context = this.GetDbContext)
            {
                return !string.IsNullOrEmpty(category) ?
                    context.Topics.Where(t => t.Category.ToString() == category).ToArray() :
                    context.Topics.ToArray();
            }
        }

        public void CreateReply(ReplyBindingModel reply)
        {
            var newReply = new Reply
            {
                ReplayDate = DateTime.Now,
                ReplayText = reply.Text,
                UserId = reply.ReplierId,
                TopicId = reply.TopicId
            };

            using (var context = this.GetDbContext)
            {
                context.Replies.Add(newReply);
                context.SaveChanges();
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