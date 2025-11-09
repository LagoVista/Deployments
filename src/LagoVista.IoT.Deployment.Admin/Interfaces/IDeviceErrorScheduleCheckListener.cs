// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 09a60fe0208db0f35d1746e5b7564801355d52661b987b3668757e0c9ef067f9
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IDeviceErrorScheduleCheckListener
    {

        Task StartAsync();
        Task StopAsync();

    }
}
