using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace rt004
{
    public static class SetUp
    {
        public static T[] GetComp<T>(string fileName)
        {
            try
            {
                string json = File.ReadAllText(fileName);
                return JsonConvert.DeserializeObject<T[]>(json, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                })!;
            }
            catch
            {
                throw new Exception("Invalid Json file!");
            }
        }

    }
}
