using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ZWaveJS.NET
{
    public class VirtualNode
    {
        internal VirtualNode(Driver driver, int[] Nodes)
        {
            _driver = driver;
            this.Nodes = Nodes;
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> GetEndpointCount()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.MCGetEndpointCount },
                { "nodeIDs", this.Nodes }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result.count").ToObject<int>());
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> SetValue(ValueID ValueID, object Value, SetValueAPIOptions Options = null)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.MCSetValue },
                { "nodeIDs", this.Nodes },
                { "valueId", ValueID },
                { "value", Value }
            };

            if (Options != null)
            {
                request.Add("options", Options);
            }

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result.result").ToObject<SetValueResult>());
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> GetDefinedValueIDs()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.MCGetDefinedValueIDs },
                { "nodeIDs", this.Nodes }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result.valueIDs").ToObject<ValueID[]>());
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> SupportsCCAPI(int CommandClass)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.MCSupportsCCAPI },
                { "nodeIDs", this.Nodes },
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
                { "command", Enums.Commands.MCInvokeCCAPI },
                { "nodeIDs", this.Nodes },
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

        private int[] Nodes { get; set; }
        private Driver _driver { get; set; }
    }

    
}
