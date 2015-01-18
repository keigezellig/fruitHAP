using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SensorProcessing.Common.Helpers
{
    public class XmlSerializerHelper
    {
        public static void Serialize<T>(string fileName, T obj)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                serializer.Serialize(fs, obj);
            }
        }

        public static T Deserialize<T>(string fileName)
        {
            T result = default(T);

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                result = (T) serializer.Deserialize(fs);
            }

            return result;

        }
    }
}
