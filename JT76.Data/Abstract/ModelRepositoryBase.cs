using System;
using System.Diagnostics;
using System.Reflection;

namespace JT76.Data.Abstract
{
    public abstract class ModelRepositoryBase
    {
        /// <summary>
        ///     Test helper class, for comparing two similar objects differing by milliseconds.
        /// </summary>
        /// <param name="modelOne"></param>
        /// <param name="modelTwo"></param>
        /// <returns></returns>
        public bool ModelEquals(object modelOne, object modelTwo)
        {
            Debug.WriteLine(GetType().FullName + "." + MethodBase.GetCurrentMethod().Name);

            Type modelOneType = modelOne.GetType();
            Type modelTwoType = modelTwo.GetType();

            if (modelOneType != modelTwoType)
                return false;

            PropertyInfo[] propertyInfoArray = modelOneType.GetProperties();
            foreach (PropertyInfo property in propertyInfoArray)
            {
                //var strPropertyName = property.Name;

                object modelOneValue = property.GetValue(modelOne, null);
                object modelTwoValue = property.GetValue(modelTwo, null);

                if (modelOneValue == null || modelTwoValue == null)
                    throw new Exception(
                        "ModelRepositoryBase.ModelEquals() was unable to get the values of an expected property");

                if (property.PropertyType == typeof (DateTime))
                {
                    if (((DateTime) modelOneValue).ToLongDateString() != ((DateTime) modelTwoValue).ToLongDateString())
                        return false;

                    if (((DateTime) modelOneValue).ToLongTimeString() != ((DateTime) modelTwoValue).ToLongTimeString())
                        return false;
                }
                else if (!modelOneValue.Equals(modelTwoValue))
                    return false;
            }

            return true;
        }
    }
}