using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JT76.Data.Abstract;
using JT76.Data.Models;

namespace JT76.Data.Database.ModelRepositories
{
    public interface ITopicRepository
    {
        bool Save();
        IQueryable<Topic> GetTopics();
        IQueryable<Topic> GetTopicsIncludingReplies();
        bool AddTopic(Topic newTopic, bool bSave);
    }

    public class TopicRepository : ModelRepositoryBase, ITopicRepository
    {
        private const int MaxTopicCount = 50;
        private const int MaxReplyCount = 50;

        private readonly JtDbContext _context;

        public TopicRepository(JtDbContext context)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _context = context;
        }

        public bool Save()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //no reason to save more than MaxCount
            if (_context.Topics.Count() > MaxTopicCount)
            {
                IEnumerable<Topic> topicsRemoved =
                    _context.Topics.RemoveRange(
                        _context.Topics.OrderByDescending(x => x.DtCreated).Take(MaxTopicCount) as DbSet<Topic>);
                _context.Topics =
                    _context.Topics.OrderByDescending(x => x.DtCreated).Take(MaxTopicCount) as DbSet<Topic>;

                foreach (Topic topic in topicsRemoved)
                {
                    int nTopicId = topic.Id;
                    _context.Replies.RemoveRange(_context.Replies.Where(x => x.TopicId == nTopicId));
                }
            }

            //no reason to save more than MaxCount
            if (_context.Replies.Count() > MaxReplyCount)
                _context.Replies =
                    _context.Replies.OrderByDescending(x => x.DtCreated).Take(MaxReplyCount) as DbSet<Reply>;

            //return that a change was made
            return (_context.SaveChanges() > 0);
        }

        public IQueryable<Topic> GetTopics()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _context.Topics;
        }


        public IQueryable<Topic> GetTopicsIncludingReplies()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //entity setup to include matching code first content
            //handles foreign keyed replies
            return _context.Topics.Include("Replies");
        }

        public bool AddTopic(Topic newTopic, bool bSave)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //return that a change was made
            _context.Topics.Add(newTopic);

            if (bSave)
                Save();

            return true;
        }


        public bool AddReply(Reply newReply)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //return that a change was made
            _context.Replies.Add(newReply);
            return true;
        }
    }
}