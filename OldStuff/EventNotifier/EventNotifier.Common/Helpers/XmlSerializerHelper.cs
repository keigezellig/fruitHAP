using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EventNotifierService.Common.Helpers
{
    public class XmlSerializerHelper
    {
        public static void Serialize<T>(string fileName, T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(new FileStream(fileName,FileMode.Create), obj);
        }

        public static T Deserialize<T>(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(new FileStream(fileName, FileMode.Open));

        }
    }
}
