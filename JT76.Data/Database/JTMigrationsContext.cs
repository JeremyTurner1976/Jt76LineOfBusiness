using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JT76.Data.Factories;
using JT76.Data.Models;

namespace JT76.Data.Database
{
    //latest version of the db
    public class JtMigrationsConfiguration : DbMigrationsConfiguration<JtDbContext>
    {
        public JtMigrationsConfiguration()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            //DANGEROUS property
            //Essentially says the data does not matter when performing a migration
            //tells entity to delete and update at will, which is dangerous for live data
            AutomaticMigrationDataLossAllowed = true;
            //DANGEROUS set to true if getting a migration exception AND you are sure the data can be scrubbed
            //THEN ensure you set this to false following the migration

            //says to use automatic migrations
            //the seed below will create a fresh db with initial data, if none exists
            AutomaticMigrationsEnabled = true;
        }


        protected override void Seed(JtDbContext context)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            base.Seed(context);

#if DEBUG
            if (!context.Topics.Any())
            {
                var topicList = JtMockFactory.GetTopicMocks();
                foreach (var topic in topicList)
                    context.Topics.Add(topic);

                context.SaveChanges();
            }

            if (!context.Errors.Any())
            {
                var errorList = JtMockFactory.GetErrorMocks();
                foreach (var error in errorList)
                    context.Errors.Add(error);

                context.SaveChanges();
            }

            if (!context.LogMessages.Any())
            {
                var logMessageList = JtMockFactory.GetLogMessageMocks();
                foreach (var logMessage in logMessageList)
                    context.LogMessages.Add(logMessage);

                context.SaveChanges();
            }
#endif
        }
    }
}