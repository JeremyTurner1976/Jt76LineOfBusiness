using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace JT76.Data.Abstract
{
    public abstract class ModelBase
    {
        //Common Properties for all items
        public int Id { get; set; }

        [CustomAttributes.DateTimeKindAttribute(DateTimeKind.Utc)]
        public DateTime DtCreated { get; set; }


        /// <summary>
        ///     Used to force a model to conform to entity requirements
        /// </summary>
        /// <returns>a valid db insertable model</returns>
        public ModelBase ForceValidData()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            Type type = GetType();

            PropertyInfo[] propertyInfoArray = type.GetProperties();
            foreach (PropertyInfo property in propertyInfoArray)
            {
                //var strPropertyName = property.Name;
                IEnumerable<CustomAttributeData> customAttributes = property.CustomAttributes;

                if (property.PropertyType != typeof (string)) continue;

                var value = property.GetValue(this, null) as string;
                if (value == null) continue;

                foreach (CustomAttributeData customAttribute in customAttributes)
                    foreach (CustomAttributeTypedArgument constructorArg in customAttribute.ConstructorArguments)
                    {
                        if (String.Equals(constructorArg.ArgumentType.Name, "Int32"))
                        {
                            int maxLength = Int32.Parse(constructorArg.Value.ToString());
                            property.SetValue(this,
                                value.Substring(0, (value.Length >= maxLength) ? maxLength : value.Length));
                        }
                    }
            }

            return this;
        }


        /// <summary>
        ///     A check on the entity class for any strings and ensures a value is set
        /// </summary>
        public bool HasNoEmptyStrings()
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            Type type = GetType();

            PropertyInfo[] propertyInfoArray = type.GetProperties();

            foreach (PropertyInfo property in propertyInfoArray)
            {
                //var strPropertyName = property.Name;
                if (property.PropertyType != typeof (string)) continue;

                var value = property.GetValue(this, null) as string;
                if (string.IsNullOrEmpty(value)) return false;
            }
            return true;
        }
    }
}