using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JT76.Data.Abstract;
using JT76.Data.Models;

namespace JT76.Data.Database.ModelRepositories
{
    public interface IReplyRepository
    {
        bool Save();
        IQueryable<Reply> GetRepliesByTopic(int topicId);
        bool AddReply(Reply newReply, bool bSave);
    }

    public class ReplyRepository : ModelRepositoryBase, IReplyRepository
    {
        private readonly JtDbContext _context;

        public ReplyRepository(JtDbContext context)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _context = context;
        }


        public bool Save()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //return that a change was made
            return (_context.SaveChanges() > 0);
        }


        public IQueryable<Reply> GetRepliesByTopic(int topicId)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _context.Replies.Where(r => r.TopicId == topicId);
        }


        public bool AddReply(Reply newReply, bool bSave)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //return that a change was made
            _context.Replies.Add(newReply);

            if (bSave)
                Save();

            return true;
        }
    }
}