using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace MySynch.Q.Common.Configurators
{
    public class SvcController: ISvcController
    {
        private readonly TimeSpan _timeOut=new TimeSpan(0,0,30);

        public void Stop( string serviceName)
        {
            ServiceController svc = new ServiceController(serviceName);
            while (ServiceStopable(svc))
            {
                svc.Stop();
                svc.WaitForStatus(ServiceControllerStatus.Stopped,_timeOut);
            }   
        }

        private static bool ServiceStopable(ServiceController serviceController)
        {
            return serviceController.Status != ServiceControllerStatus.Stopped && serviceController.Status != ServiceControllerStatus.StopPending && serviceController.CanStop && serviceController.CanShutdown;
        }

        public void Start(string serviceName)
        {
            ServiceController svc = new ServiceController(serviceName);
            if (ServiceStartable(svc))
            {
                svc.Start();
                svc.WaitForStatus(ServiceControllerStatus.Running,_timeOut);
            }
        }

        private bool ServiceStartable(ServiceController serviceController)
        {
            return serviceController.Status != ServiceControllerStatus.Running && serviceController.Status != ServiceControllerStatus.StartPending;
        }

        public void Stop(IEnumerable<string> serviceNames)
        {
            foreach(var serviceName in serviceNames)
                Stop(serviceName);
        }

        public void Start(IEnumerable<string> serviceNames)
        {
            foreach (var serviceName in serviceNames)
                Start(serviceName);
        }
    }
}
