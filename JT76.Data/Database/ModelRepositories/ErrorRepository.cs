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

    public interface IErrorRepository
    {
        bool Save();
        IQueryable<Error> GetErrors();
        bool AddError(Error newError, bool bSave);
    }

    public class ErrorRepository : ModelRepositoryBase, IErrorRepository
    {
        private const int MaxErrorCount = 50;

        private readonly JtDbContext _context;

        public ErrorRepository(JtDbContext context)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            _context = context;
        }

        public bool Save()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //no reason to save more than MaxCount
            if (_context.Errors.Count() > MaxErrorCount)
                _context.Errors =
                    _context.Errors.OrderByDescending(x => x.DtCreated).Take(MaxErrorCount) as DbSet<Error>;

            //return that a change was made
            return (_context.SaveChanges() > 0);
        }

        public IQueryable<Error> GetErrors()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            return _context.Errors;
        }

        public bool AddError(Error newError, bool bSave)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //want to force this to hit the db if the model is invalid
            newError = (Error) newError.ForceValidData();

            _context.Errors.Add(newError);

            if (bSave)
                Save();

            return true;
        }
    }
}