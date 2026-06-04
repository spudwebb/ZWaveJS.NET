using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWaveJS.NET
{
    public class MethodFactory
    {
        public static Func<Dictionary<string, object>, Task<CMDResult>> CreateVOID(Driver Runtime, string ServerMethod)
        {
            return (x) => Execute(Runtime, ServerMethod, x);
        }

        public static Func<Dictionary<string, object>, Task<CMDResult>> CreatePRIMITIVE(Driver Runtime, string ServerMethod, string ObjectPath)
        {
            return (x) => Execute(Runtime, ServerMethod, x, ObjectPath);
        }

        public static Func<Dictionary<string, object>, Task<CMDResult>> CreateCLASS(Driver Runtime, string ServerMethod, Type MappedClass, string ObjectPath)
        {
            return (x) => Execute(Runtime, ServerMethod, x, MappedClass, ObjectPath);
        }

        private static Task<CMDResult> Execute(Driver Runtime, string ServerMethod, Dictionary<string, object> Args, string ObjectPath)
        {
            // create a shallow copy to avoid mutating caller dictionary
            var request = new Dictionary<string, object>(Args ?? new Dictionary<string, object>());

            // ensure command is set as expected by the server
            request.Remove("messageId");
            request["command"] = ServerMethod;

            return Runtime.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    object Obj = JO.SelectToken(ObjectPath).ToObject<object>();
                    Res.SetPayload(Obj);
                }
            });
        }

        private static Task<CMDResult> Execute(Driver Runtime, string ServerMethod, Dictionary<string, object> Args, Type MappedClass, string ObjectPath)
        {
            var request = new Dictionary<string, object>(Args ?? new Dictionary<string, object>());

            request.Remove("messageId");
            request["command"] = ServerMethod;

            return Runtime.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    object Obj = JO.SelectToken(ObjectPath).ToObject(MappedClass);
                    Res.SetPayload(Obj);
                }
            });
        }

        private static Task<CMDResult> Execute(Driver Runtime, string ServerMethod, Dictionary<string, object> Args)
        {
            var request = new Dictionary<string, object>(Args ?? new Dictionary<string, object>());

            request.Remove("messageId");
            request["command"] = ServerMethod;

            return Runtime.SendRequestAsync(request);
        }
    }
}
