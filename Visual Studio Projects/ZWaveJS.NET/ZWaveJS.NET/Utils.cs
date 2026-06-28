using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using System.Collections.Generic;

namespace ZWaveJS.NET
{

    public class Utils
    {
        private Driver _driver;
        internal Utils(Driver Driver)
        {
            _driver = Driver;
        }

        // Checked as of : 3.5.0
        public Task<CMDResult> ParseQRCodeString(string QR)
        {
            var request = new Dictionary<string, object>
            {
                { "command", Enums.Commands.ParseQRCodeString },
                { "qr", QR }
            };

            return _driver.SendRequestAsync(request, (JO, Res) =>
            {
                if (Res.Success)
                {
                    QRProvisioningInformation PQR = JO.SelectToken("result.qrProvisioningInformation").ToObject<QRProvisioningInformation>();
                    Res.SetPayload(PQR);
                }
            });
        }
    }
}
