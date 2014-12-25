using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace JT76.Data
{
    //These attributes must be set in the dbcontext ctor
    public class CustomAttributes
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class CleanedHtmlString : Attribute
        {
            public static void Apply(object entity)
            {
                Debug.WriteLine("CleanedHtmlString.Apply()");

                if (entity == null)
                    return;

                IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties()
                    .Where(x => x.PropertyType == typeof (string));

                foreach (PropertyInfo property in properties)
                {
                    var strItem = (string) property.GetValue(entity);

                    string[] lines = strItem.Split(new[] {Environment.NewLine}, StringSplitOptions.None).ToArray();
                    string cleanedHtmlString = string.Join("<br/>", lines);

                    property.SetValue(entity, cleanedHtmlString);
                }
            }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class DateTimeKindAttribute : Attribute
        {
            private readonly DateTimeKind _kind;

            public DateTimeKindAttribute(DateTimeKind kind)
            {
                Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

                _kind = kind;
            }

            public DateTimeKind Kind
            {
                get { return _kind; }
            }

            public static void Apply(object entity)
            {
                Debug.WriteLine("DateTimeKindAttribute.Apply()");

                if (entity == null)
                    return;

                IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties()
                    .Where(x => x.PropertyType == typeof (DateTime) || x.PropertyType == typeof (DateTime?));

                foreach (PropertyInfo property in properties)
                {
                    var attr = property.GetCustomAttribute<DateTimeKindAttribute>();
                    if (attr == null)
                        continue;

                    DateTime? dt = property.PropertyType == typeof (DateTime?)
                        ? (DateTime?) property.GetValue(entity)
                        : (DateTime) property.GetValue(entity);

                    if (dt == null)
                        continue;

                    property.SetValue(entity, DateTime.SpecifyKind(dt.Value, attr.Kind));
                }
            }
        }
    }
}