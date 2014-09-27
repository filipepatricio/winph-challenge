using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DesafioThingPink
{
    class JsonUtil
    {
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            byte[] ms_bytes = ms.ToArray();
            string retVal = Encoding.UTF8.GetString(ms_bytes, 0, ms_bytes.Length);
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            Stream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            try
            {
                obj = (T)serializer.ReadObject(ms);
            }
            catch (Exception ex) { }
            return obj;
        }

    }
}
