using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using JT76.Data.Models;
using Newtonsoft.Json.Linq;

namespace JT76.Data.Database.ModelRepositories
{
    class LineOfBusinessRepository
    {
        private readonly EFContextProvider<JtDbContext> _contextProvider = new EFContextProvider<JtDbContext>();

        private JtDbContext Context { get { return _contextProvider.Context; } }

        public string Metadata
        {
            get { return _contextProvider.Metadata(); }
        }

        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _contextProvider.SaveChanges(saveBundle);
        }

        public IQueryable<Session> Sessions
        {
            get { return Context.Sessions; }
        }

        public IQueryable<Person> Speakers
        {
            get { return Context.Persons.Where(p => p.SpeakerSessions.Any()); }
        }

        public IQueryable<Person> Persons
        {
            get { return Context.Persons; }
        }

        public IQueryable<Room> Rooms
        {
            get { return Context.Rooms; }
        }

        public IQueryable<TimeSlot> TimeSlots
        {
            get { return Context.TimeSlots; }
        }

        public IQueryable<Track> Tracks
        {
            get { return Context.Tracks; }
        }
    }
}
