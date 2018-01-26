using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Sender.ConfigurationProvider
{
    public interface ISenderConfigurationProvider
    {
        void Load(string fileName);

        bool Save(string fileName);

        bool SaveAndRestart(string fileName);
    }
}
