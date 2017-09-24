namespace Blog.Services.DataServices
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Models.BindingModels;
    using Models.ViewModels;

    public class ReplyDataService : BaseDataService
    {
        public Task<ReplyViewModel[]> GetRepliesByTopicId(int topicId)
        {
            return this.GetDbContext
                .Replies
                .Include(r => r.User)
                .Include(t => t.Topic)
                .Where(r => r.TopicId == topicId)
                .SelectMany(r => new[]
                {
                    new ReplyViewModel
                    {
                        Id = r.Id,
                        ReplierId = r.UserId,
                        ReplyDate = r.ReplayDate,
                        ReplyText = r.ReplayText,
                        ReplierUserName = r.User.UserName
                    }
                }).ToArrayAsync();
        }

        public Task CreateReply(ReplyBindingModel reply)
        {
            var newReply = new Reply
            {
                ReplayDate = DateTime.Now,
                ReplayText = reply.Text,
                UserId = reply.ReplierId,
                TopicId = reply.TopicId
            };

            var context = this.GetDbContext;
            context.Replies.Add(newReply);
            return context.SaveChangesAsync();
        }

        public Task DeleteReply(int replyId)
        {
            var context = this.GetDbContext;
            context.Replies.Remove(context.Replies.Find(replyId));
            return context.SaveChangesAsync();
        }

        public async Task<bool> ReplyExists(int replyId)
        {
            return await Task.FromResult(this.GetReplyById(replyId) != null);
        }

        public Task<Reply> GetReplyById(int replyId)
        {
            return Task.FromResult(this.GetDbContext.Replies.Find(replyId));
        }

        public void UpdateModel(ReplyBindingModel rbm)
        {
            var date = this.GetDbContext.Replies.Find(rbm.ReplyId).ReplayDate;
            var newReply = new Reply
            {
                Id = rbm.ReplyId,
                UserId = rbm.ReplierId,
                ReplayText = rbm.Text,
                TopicId = rbm.TopicId,
                ReplayDate = date
            };
            using (var context = this.GetDbContext)
            {
                context.Replies.AddOrUpdate(newReply);
                context.SaveChanges();
            }
        }

        public void UpdateModelByModal(Reply reply)
        {
            var newReply = new Reply
            {
                Id = reply.Id,
                UserId = reply.UserId,
                ReplayText = reply.ReplayText,
                TopicId = reply.TopicId,
                ReplayDate = reply.ReplayDate
            };
            using (var context = this.GetDbContext)
            {
                context.Replies.AddOrUpdate(newReply);
                context.SaveChanges();
            }
        }
    }
}