using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JT76.Data.Database.ModelRepositories;
using JT76.Data.Models;

namespace JT76.Ui.ViewModels
{
    public class MessageBoardViewModel
    {
        private readonly IReplyRepository _replyRepo;
        private readonly ITopicRepository _topicRepo;

        public MessageBoardViewModel(ITopicRepository topicRepo, IReplyRepository replyRepo)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _topicRepo = topicRepo;
            _replyRepo = replyRepo;
        }

        public bool Save()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return (_replyRepo.Save() && _topicRepo.Save());
        }

        public IQueryable<Topic> GetTopics()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _topicRepo.GetTopics();
        }

        public IQueryable<Topic> GetTopicsIncludingReplies()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _topicRepo.GetTopicsIncludingReplies();
        }

        public IQueryable<Reply> GetRepliesByTopic(int topicId)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _replyRepo.GetRepliesByTopic(topicId);
        }

        public bool AddTopic(Topic newTopic)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _topicRepo.AddTopic(newTopic, true);
            return true;
        }

        public bool AddReply(Reply newReply)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _replyRepo.AddReply(newReply, true);
            return true;
        }
    }
}