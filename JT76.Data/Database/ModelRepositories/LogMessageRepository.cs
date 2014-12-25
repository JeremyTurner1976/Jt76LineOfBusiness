using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JT76.Data.Abstract;
using JT76.Data.Models;

namespace JT76.Data.Database.ModelRepositories
{
    //Production (TimedService) 
    //Calls this web.config db and runs procs to run db maintenance daily

    public interface ILogMessageRepository
    {
        bool Save();
        IQueryable<LogMessage> GetLogMessages();
        bool AddLogMessage(LogMessage logMessage, bool bSave);
    }

    public class LogMessageRepository : ModelRepositoryBase, ILogMessageRepository
    {
        private const int MaxLogMessageCount = 50;

        private readonly JtDbContext _context;

        public LogMessageRepository(JtDbContext context)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _context = context;
        }

        public bool Save()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //no reason to save more than MaxCount
            if (_context.LogMessages.Count() > MaxLogMessageCount)
                _context.LogMessages =
                    _context.LogMessages.OrderByDescending(x => x.DtCreated).Take(MaxLogMessageCount) as
                        DbSet<LogMessage>;

            //return that a change was made
            return (_context.SaveChanges() > 0);
        }

        public IQueryable<LogMessage> GetLogMessages()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _context.LogMessages;
        }

        public bool AddLogMessage(LogMessage logMessage, bool bSave)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //want to force this to hit the db if the model is invalid
            logMessage = (LogMessage) logMessage.ForceValidData();

            _context.LogMessages.Add(logMessage);

            if (bSave)
                Save();

            return true;
        }
    }
}