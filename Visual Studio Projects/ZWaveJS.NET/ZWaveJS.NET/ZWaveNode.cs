using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using static ZWaveJS.NET.Enums;

namespace ZWaveJS.NET
{
    public class ZWaveNode
    {
        private Driver _driver;
        internal ZWaveNode(Driver driver = null)
        {
            _driver = driver;
        }
        
        public delegate void LifelineHealthCheckProgress(int Round, int TotalRounds, int LastRating);
        private LifelineHealthCheckProgress LifelineHealthCheckProgressSub;
        internal void Trigger_LifelineHealthCheckProgress(int Round, int TotalRounds, int LastRating)
        {
            LifelineHealthCheckProgressSub?.Invoke(Round, TotalRounds, LastRating);
        }

        public delegate void StatisticsUpdatedEvent(ZWaveNode Node, NodeStatisticsUpdatedArgs Args);
        public event StatisticsUpdatedEvent StatisticsUpdated;
        internal void Trigger_StatisticsUpdated(NodeStatisticsUpdatedArgs Args)
        {
            this.statistics = Args;
            StatisticsUpdated?.Invoke(this, Args);
        }

        public delegate void FirmwareUpdateFinishedEvent(ZWaveNode Node, NodeFirmwareUpdateResultArgs Args);
        public event FirmwareUpdateFinishedEvent FirmwareUpdateFinished;
        internal void Trigger_FirmwareUpdateFinished(NodeFirmwareUpdateResultArgs Args)
        {
            FirmwareUpdateFinished?.Invoke(this, Args);
        }

        public delegate void FirmwareUpdateProgressEvent(ZWaveNode Node, NodeFirmwareUpdateProgressArgs Args);
        public event FirmwareUpdateProgressEvent FirmwareUpdateProgress;
        internal void Trigger_FirmwareUpdateProgress(NodeFirmwareUpdateProgressArgs Args)
        {
            FirmwareUpdateProgress?.Invoke(this, Args);
        }

        public delegate void ValueNotificationEvent(ZWaveNode Node, ValueNotificationArgs Args);
        public event ValueNotificationEvent ValueNotification;
        internal void Trigger_ValueNotification(ValueNotificationArgs Args)
        {
            ValueNotification?.Invoke(this, Args);
        }

        public delegate void MetadataUpdatedEvent(ZWaveNode Node, MetadataUpdatedArgs Args);
        public event MetadataUpdatedEvent MetadataUpdated;
        internal void Trigger_MetadataUpdated(MetadataUpdatedArgs Args)
        {
            MetadataUpdated?.Invoke(this, Args);
        }

        public delegate void ValueUpdatedEvent(ZWaveNode Node, ValueUpdatedArgs Args);
        public event ValueUpdatedEvent ValueUpdated;
        internal void Trigger_ValueUpdated(ValueUpdatedArgs Args)
        {
            ValueUpdated?.Invoke(this, Args);
        }

        public delegate void ValueAddedEvent(ZWaveNode Node, ValueAddedArgs Args);
        public event ValueAddedEvent ValueAdded;
        internal void Trigger_ValueAdded(ValueAddedArgs Args)
        {
            ValueAdded?.Invoke(this, Args);
        }

        public delegate void ValueRemovedEvent(ZWaveNode Node, ValueRemovedArgs Args);
        public event ValueRemovedEvent ValueRemoved;
        internal void Trigger_ValueRemoved(ValueRemovedArgs Args)
        {
            ValueRemoved?.Invoke(this, Args);
        }

        public delegate void NotificationEvent(ZWaveNode Node, int ccId, JObject Args);
        public event NotificationEvent Notification;
        internal void Trigger_Notification(int CCID, JObject Args)
        {
            Notification?.Invoke(this, CCID, Args);
        }
        
        public delegate void NodeInfoEvent(ZWaveNode Node);
        public event NodeInfoEvent NodeInfo;
        internal void Trigger_NodeInfo()
        {
            NodeInfo?.Invoke(this);
        }

        public delegate void NodeAliveEvent(ZWaveNode Node);
        public event NodeAliveEvent NodeAlive;
        internal void Trigger_NodeAlive()
        {
            this.status = Enums.NodeStatus.Alive;
            NodeAlive?.Invoke(this);
        }

        public delegate void NodeDeadEvent(ZWaveNode Node);
        public event NodeDeadEvent NodeDead;
        internal void Trigger_NodeDead()
        {
            this.status = Enums.NodeStatus.Dead;
            NodeDead?.Invoke(this);
        }

        public delegate void AwakeEvent(ZWaveNode Node);
        public event AwakeEvent NodeAwake;
        internal void Trigger_NodeAwake()
        {
            this.status = Enums.NodeStatus.Awake;
            NodeAwake?.Invoke(this);
        }

        public delegate void SleepEvent(ZWaveNode Node);
        public event SleepEvent NodeAsleep;
        internal void Trigger_NodeAsleep()
        {
            this.status = Enums.NodeStatus.Asleep;
            NodeAsleep?.Invoke(this);
        }

        public delegate void NodeReadyEvent(ZWaveNode Node);
        public event NodeReadyEvent NodeReady;
        internal void Trigger_NodeReady()
        {
            this.ready = true;
            NodeReady?.Invoke(this);
        }

        public delegate void NodeInterviewStartedEvent(ZWaveNode Node);
        public event NodeInterviewStartedEvent NodeInterviewStarted;
        internal void Trigger_NodeInterviewStarted()
        {
            NodeInterviewStarted?.Invoke(this);
        }

        public delegate void NodeInterviewCompletedEvent(ZWaveNode Node);
        public event NodeInterviewCompletedEvent NodeInterviewCompleted;
        internal void Trigger_NodeInterviewCompleted()
        {
            NodeInterviewCompleted?.Invoke(this);
        }

        public delegate void NodeInterviewFailedEvent(ZWaveNode Node, NodeInterviewFailedEventArgs Args);
        public event NodeInterviewFailedEvent NodeInterviewFailed;
        internal void Trigger_NodeInterviewFailed(NodeInterviewFailedEventArgs Args)
        {
            NodeInterviewFailed?.Invoke(this, Args);
        }
        
        // Checked as of : 3.5.0
        public Task<CMDResult> Ping()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.Ping },
                { "nodeId", this.id }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result.responded").ToObject<bool>());
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> Interview()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.Interview },
                { "nodeId", this.id }
            };

            return _driver.SendRequestAsync(request);
        }
        
        // Checked as of : 3.5.0
        public Task<CMDResult> CheckLifelineHealth(int Rounds, LifelineHealthCheckProgress OnProgress = null)
        {
            LifelineHealthCheckProgressSub = OnProgress;

            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.CheckLifelineHealth },
                { "nodeId", this.id },
                { "rounds", Rounds }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    LifelineHealthCheckSummary LLHCS = JO.SelectToken("result.summary").ToObject<LifelineHealthCheckSummary>();
                    Res.SetPayload(LLHCS);
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> AbortFirmwareUpdate()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.AbortFirmwareUpdate },
                { "nodeId", this.id }
            };

            return _driver.SendRequestAsync(request);
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> UpdateFirmware(FirmwareUpdate[] Updates)
        {
            foreach(FirmwareUpdate FWU in Updates)
            {
                if(FWU.firmwareTarget == null)
                {
                    CMDResult Res = new CMDResult(Enums.ErrorCodes.WrongOverride, "Please use the override that includes 'firmwareTarget'", false);
                    return Task.FromResult(Res);
                }
            }

            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.UpdateFirmware },
                { "nodeId", this.id },
                { "updates", Updates }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result.result").ToObject<NodeFirmwareUpdateResultArgs>());
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> RefreshInfo(RefreshInfoOptions Options = null)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.RefreshInfo },
                { "nodeId", this.id }
            };

            if (Options != null)
                request.Add("options", Options);

            return _driver.SendRequestAsync(request);
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> GetValue(ValueID ValueID)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.GetValue },
                { "valueId", ValueID },
                { "nodeId", this.id }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result").ToObject<JObject>());
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> SetValue(ValueID ValueID, object Value, SetValueAPIOptions Options = null)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.SetValue },
                { "nodeId", this.id },
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
                    SetValueResult SVR = JO.SelectToken("result.result").ToObject<SetValueResult>();
                    Res.SetPayload(SVR);
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> PollValue(ValueID ValueID)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.PollValue },
                { "nodeId", this.id },
                { "valueId", ValueID }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result").ToObject<JObject>());
                }
            });
        }

        // Checked as of : 3.5.0 - Variant 1: Normal parameter, defined in a config file
        public Task<CMDResult> ZWJSS_SetRawConfigParameterValue(int Parameter, int Value)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.SetRawConfigParameterValue },
                { "nodeId", this.id },
                { "parameter", Parameter },
                { "value", Value }
            };

            return _driver.SendRequestAsync(request);
        }

        // Checked as of : 3.5.0 - Variant 2: Normal parameter, not defined in a config file
        public Task<CMDResult> ZWJSS_SetRawConfigParameterValue(int Parameter, int Value, int ValueSize, Enums.ConfigValueFormat ValueFormat)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.SetRawConfigParameterValue },
                { "nodeId", this.id },
                { "parameter", Parameter },
                { "value", Value },
                { "valueSize", ValueSize },
                { "valueFormat", ValueFormat }
            };

            return _driver.SendRequestAsync(request);
        }

        // Checked as of : 3.5.0 - Variant 3: Partial parameter, must be defined in a config file
        public Task<CMDResult> ZWJSS_SetRawConfigParameterValue(int Parameter, int Bitmask, int Value)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.SetRawConfigParameterValue },
                { "nodeId", this.id },
                { "parameter", Parameter },
                { "bitMask", Bitmask },
                { "value", Value }
            };

            return _driver.SendRequestAsync(request);
        }
        
        // Checked as of : 3.5.0
        public Task<CMDResult> RefreshValues()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.RefreshValues },
                { "nodeId", this.id }
            };

            return _driver.SendRequestAsync(request);
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> RefreshCCValues(int CommandClass)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.RefreshCCValues },
                { "commandClass", CommandClass },
                { "nodeId", this.id }
            };

            return _driver.SendRequestAsync(request);
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> GetDefinedValueIDs()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.GetDefinedValueIDs },
                { "nodeId", this.id }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result.valueIds").ToObject<ValueID[]>());
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> GetValueMetadata(ValueID VID)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.GetValueMetadata },
                { "nodeId", this.id },
                { "valueId", VID }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result").ToObject<ValueMetadata>());
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> SupportsCCAPI(int CommandClass)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.SupportsCCAPI },
                { "nodeId", this.id },
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
                { "nodeId", this.id },
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

       

        // Checked as of : 3.5.0
        public Task<CMDResult> GetEndpointCount()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.GetEndpointCount },
                { "nodeId", this.id }
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
        public Task<CMDResult> GetHighestSecurityClass()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.GetHighestSecurityClass },
                { "nodeId", this.id }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Enums.SecurityClass Value = JO.SelectToken("result.highestSecurityClass").ToObject<Enums.SecurityClass>();
                    Res.SetPayload(Value);
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> HasSecurityClass(Enums.SecurityClass Class)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.HasSecurityClass },
                { "nodeId", this.id },
                { "securityClass", Class }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    Res.SetPayload(JO.SelectToken("result.hasSecurityClass").ToObject<bool>());
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> WaitForWakeup()
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.WaitForWakeUp },
                { "nodeId", this.id }
            };

            return _driver.SendRequestAsync(request);
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> ManuallyIdleNotificationValue(ValueID VID)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.ManuallyIdleNotificationValue },
                { "nodeId", this.id },
                { "valueId", VID }
            };

            return _driver.SendRequestAsync(request);
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> ManuallyIdleNotificationValue(int notificationType, int prevValue, int? endpointIndex = null)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.ManuallyIdleNotificationValue },
                { "nodeId", this.id },
                { "notificationType", notificationType },
                { "prevValue", prevValue }
            };

            if (endpointIndex.HasValue)
            {
                request.Add("endpointIndex", endpointIndex.Value);
            }

            return _driver.SendRequestAsync(request);
        }

         // LOCAL
        public Endpoint GetEndpoint(int Index)
        {
            Endpoint EP = this.endpoints.FirstOrDefault((E) => E.index.Equals(Index));
            return EP;
        }

        [Newtonsoft.Json.JsonProperty]
        public Endpoint[] endpoints { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public bool isControllerNode { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public Enums.NodeStatus status { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public bool ready { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public bool isListening { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public bool isRouting { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public bool isSecure { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int manufacturerId { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int productId { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int productType { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public string firmwareVersion { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public string zwavePlusVersion { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public DeviceConfig deviceConfig { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public object isFrequentListening { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public long maxDataRate { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public long[] supportedDataRates { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int protocolVersion { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public bool supportsBeaming { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public bool supportsSecurity { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int zwavePlusNodeType { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int zwavePlusRoleType { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public DeviceClass deviceClass { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public string interviewStage { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public string deviceDatabaseUrl { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int interviewAttempts { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public string label { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public int nodeType { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public CommandClass[] commandClasses { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public NodeStatistics statistics { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public DateTime? lastSeen { get; internal set; }
        [Newtonsoft.Json.JsonProperty]
        public Protocols protocol { get; internal set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "nodeId")]
        public int id { get; internal set; }
        
        [Newtonsoft.Json.JsonProperty]
        public bool keepAwake { get; internal set; }
        public Task<CMDResult>  SetKeepAwake(bool Option)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.KeepNodeAwake },
                { "nodeId", this.id },
                { "keepAwake", Option }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    this.keepAwake = Option;
                }
            });
        }

        [Newtonsoft.Json.JsonProperty]
        public string name { get; internal set; }
        public Task<CMDResult> SetName(string Name, bool UpdateCC = true)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.SetName },
                { "nodeId", this.id },
                { "name", Name },
                { "updateCC", UpdateCC }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    this.name = Name;
                }
            });
        }

        [Newtonsoft.Json.JsonProperty]
        public string location { get; internal set; }
        public Task<CMDResult> SetLocation(string Location, bool UpdateCC = true)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.SetLocation },
                { "nodeId", this.id },
                { "location", Location },
                { "updateCC", UpdateCC }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    this.location = Location;
                }
            });
        }

    }
}
