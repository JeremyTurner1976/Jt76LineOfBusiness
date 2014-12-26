using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Reflection;
using JT76.Data.Models;

namespace JT76.Data.Database
{
    public interface IDbContext : IDisposable
    {
    }

    public class JtDbContext : DbContext, IDbContext
    {
        public JtDbContext()
            : base("JtConnection")
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //anything that is lazy loaded attempts to generate the object
            //so turn this off and eager load instead
            Configuration.LazyLoadingEnabled = false;

            //handles the change manager in a very straight forward way
            Configuration.ProxyCreationEnabled = false;

            //Code First Migration Flag
            System.Data.Entity.Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<JtDbContext, JtMigrationsConfiguration>());

            //Implement any custom attributes, this will be null when mocking the db context
            ObjectContext objectContext = ((IObjectContextAdapter) this).ObjectContext;
            if (objectContext != null)
            {
                objectContext.ObjectMaterialized +=
                    (sender, e) => CustomAttributes.DateTimeKindAttribute.Apply(e.Entity);
                objectContext.ObjectMaterialized +=
                    (sender, e) => CustomAttributes.CleanedHtmlString.Apply(e.Entity);
            }
        }

        //This will force some of the db constraints to be parts of the model
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //this will set the max length of the property the same as it is on the db
            //modelBuilder.Properties<string>().Configure(p => p.IsMaxLength());

            // Use singular table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new SessionConfiguration());
            modelBuilder.Configurations.Add(new AttendanceConfiguration());
        }



        //Message Board
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<Reply> Replies { get; set; }

        //Error
        public virtual DbSet<Error> Errors { get; set; }

        //LogMessage
        public virtual DbSet<LogMessage> LogMessages { get; set; }

        //Angular Line of Business
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Attendance> Attendance { get; set; }

        // Lookup Lists
        public DbSet<Room> Rooms { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Track> Tracks { get; set; }
    }
}