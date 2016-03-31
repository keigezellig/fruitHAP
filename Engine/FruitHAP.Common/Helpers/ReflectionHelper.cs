using System;
using System.Reflection;

namespace FruitHAP.Common.Helpers
{
	public static class ReflectionHelper
	{

		public static void SetProperty(this object obj, string propertyName, object value)
		{
			PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			if (prop == null)
			{
				throw new ArgumentException (string.Format ("Public property {0} not found on instance", propertyName));
			}
			if (!prop.CanWrite) 
			{
				throw new ArgumentException (string.Format ("Public property {0} has only a getter", propertyName));
			}
				
			var valueOfCorrectType = ConvertType (value, prop.PropertyType);
			prop.SetValue (obj, valueOfCorrectType);
		}

        public static object GetProperty(this object obj, string propertyName)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
            {
                throw new ArgumentException (string.Format ("Public property {0} not found on instance", propertyName));
            }
            if (!prop.CanRead) 
            {
                throw new ArgumentException (string.Format ("Public property {0} has only a setter", propertyName));
            }
                
            return prop.GetValue(obj);
        }


		private static object ConvertType(object value, Type type)
		{
			if (type.IsEnum) 
			{
				if (value.GetType() == typeof(string)) 
				{
					return Enum.Parse (type, value.ToString());	
				}
				return Enum.ToObject(type, value);
			}

			return Convert.ChangeType(value, type);
		}
	}
}

