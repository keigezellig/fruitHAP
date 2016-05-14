using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Common.Helpers
{
    public static class AssemblyHelpers
    {
        public static string GetAssemblyDirectory(Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.Location);
        }

        public static string GetAssemblyVersion(Assembly assembly)
        {
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            if (attributes.Length > 0)
            {
                var titleAttribute = (AssemblyInformationalVersionAttribute)attributes[0];
                if (titleAttribute.InformationalVersion.Length > 0)
                    return titleAttribute.InformationalVersion;
            }

            return "";
        }

        public static string GetAssemblyTitle(Assembly assembly)
        {
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                if (titleAttribute.Title.Length > 0)
                    return titleAttribute.Title;
            }

            return "";
        }

        public static string GetAssemblyDescription(Assembly assembly)
        {
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                var descriptionAttribute = (AssemblyDescriptionAttribute)attributes[0];
                if (descriptionAttribute.Description.Length > 0)
                    return descriptionAttribute.Description;
            }

            return "";
        }
    }
}
