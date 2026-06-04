using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ZWaveJS.NET
{
    public class Endpoint
    {
        private Driver _driver;
        internal Endpoint(Driver driver = null)
        {
            _driver = driver;
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> SupportsCCAPI(int CommandClass)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.SupportsCCAPI },
                { "nodeId", this.nodeId },
                { "endpoint", this.index },
                { "commandClass", CommandClass }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result.supported").ToObject<bool>());
                }
            });
        }


        // Checked as of : 3.5.0
        public Task<CMDResult> InvokeCCAPI(int CommandClass, string Method, params object[] Params)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.InvokeCCAPI },
                { "nodeId", this.nodeId },
                { "endpoint", this.index },
                { "commandClass", CommandClass },
                { "methodName", Method },
                { "args", Params }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result").ToObject<JObject>());
                }
            });
        }

        public Task<CMDResult> GetCCs()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.GetCCs },
                { "nodeId", this.nodeId },
                { "endpoint", this.index }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result.commandClasses").ToObject<Dictionary<int, CommandClassInfo>>());
                }
            });
        }

        [Newtonsoft.Json.JsonProperty]
        public int nodeId { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int index { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int installerIcon { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int userIcon { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public DeviceClass deviceClass { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public CommandClass[] commandClasses { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public string endpointLabel { get; internal set; }
    }
}
