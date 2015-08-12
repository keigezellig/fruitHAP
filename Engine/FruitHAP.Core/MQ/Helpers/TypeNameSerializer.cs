using System;
using EasyNetQ;
using System.Reflection;

namespace FruitHAP.Core.MQ.Helpers
{
	public class TypeNameSerializer : ITypeNameSerializer
	{
		public string Serialize (Type type)
		{
			return type.FullName;
		}
		public Type DeSerialize (string typeName)
		{
			Assembly assembly = Assembly.GetCallingAssembly ();
			var type = Type.GetType(typeName + ", " + assembly.FullName);

			if (type == null)
			{
				throw new EasyNetQException(
					"Cannot find type {0} in {1}",
					typeName, assembly.FullName);
			}
			return type;

		}
	}
}

