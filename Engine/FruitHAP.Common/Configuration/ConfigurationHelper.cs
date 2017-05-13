using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace FruitHAP.Common.Configuration
{
    public static class ConfigurationHelper
    {
        public static bool IsPropertyAConfigurableItem(this object obj, string propertyName, bool onlySpecific=false)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
            {
                return false;
            }

            Attribute[] attrs = Attribute.GetCustomAttributes(prop);

            var attr = attrs.SingleOrDefault(a => (a is ConfigurationItemAttribute));
            if (attr != null)
            {
                if (onlySpecific)
                {
                    return (attr as ConfigurationItemAttribute).IsSensorSpecific == onlySpecific;
                }
                else
                {
                    return true;
                }
            }

            return false;

        }

        public static List<PropertyInfo> GetConfigurableProperties(this object obj, bool onlySpecific=false)
        {
            return obj.GetType().GetProperties().Where(prop => obj.IsPropertyAConfigurableItem(prop.Name,onlySpecific)).ToList();
        }
    }
}

