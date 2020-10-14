using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Debouncer
{
    public static class JsonFile
    {
        public static void WriteObjectToFile<T>(this T obj, string path)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public static T LoadObjectFromFile<T>(string path)
        {
            T res;
            try
            {
                res = JsonConvert.DeserializeObject<T>(File.ReadAllText(path), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, MethodBase.GetCurrentMethod().Name);
                throw;
            }

            return res;
        }
    }
}