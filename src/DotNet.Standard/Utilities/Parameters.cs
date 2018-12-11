using System;
using System.Reflection;

namespace DotNet.Standard.Utilities
{
    public static class Parameters
    {
        public static bool Exists(this Type type, string propertyName, out PropertyInfo propertyInfo)
        {
            foreach (PropertyInfo property in type.GetProperties())
            {
                Type t = property.PropertyType.ToBasic();
                if (t.IsSystem() && property.Name == propertyName)
                {
                    propertyInfo = property;
                    return true;
                }
            }
            propertyInfo = null;
            return false;
        }
    }
}