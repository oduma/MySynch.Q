using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Common.Configurators
{
    public interface ISvcController
    {
        void Stop(IEnumerable<string> serviceNames);
        void Stop(string serviceName);

        void Start(IEnumerable<string> serviceNames);
        void Start(string serviceName);
    }
}
