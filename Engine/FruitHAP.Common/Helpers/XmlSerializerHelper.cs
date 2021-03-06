﻿using System.IO;
using System.Xml.Serialization;

namespace FruitHAP.Common.Helpers
{
    public static class XmlSerializerHelper
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
