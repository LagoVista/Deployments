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
