using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using System.Collections.Generic;

namespace ZWaveJS.NET
{
    public class ConfigManager
    {
        private Driver _driver;
        internal ConfigManager(Driver Driver)
        {
            _driver = Driver;

            // Fire-and-forget the load commands but observe failures to avoid unobserved exceptions.
            var req1 = new Dictionary<string, object>
            {
                { "command", Enums.Commands.LoadManufacturers }
            };
            _ = _driver.SendRequestAsync(req1).ContinueWith(t =>
            {
                System.Diagnostics.Debug.WriteLine($"LoadManufacturers request failed: {t.Exception?.GetBaseException()}");
            }, TaskContinuationOptions.OnlyOnFaulted);

            var req2 = new Dictionary<string, object>
            {
                { "command", Enums.Commands.LoadDeviceIndex }
            };
            _ = _driver.SendRequestAsync(req2).ContinueWith(t =>
            {
                System.Diagnostics.Debug.WriteLine($"LoadDeviceIndex request failed: {t.Exception?.GetBaseException()}");
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> LookupDevice(int ManufacturerID, int ProductTypeID, int ProductId)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.LookupDevice },
                { "manufacturerId", ManufacturerID },
                { "productType", ProductTypeID },
                { "productId", ProductId }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    DeviceConfig Config = JO.SelectToken("result.config").ToObject<DeviceConfig>();
                    Res.SetPayload(Config);
                }
            });
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> LookupManufacturer(int ManufacturerID)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.LookupManufacturer },
                { "manufacturerId", ManufacturerID }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    string Name = JO.SelectToken("result.name").ToObject<string>();
                    Res.SetPayload(Name);
                }
            });
        }
    }
}