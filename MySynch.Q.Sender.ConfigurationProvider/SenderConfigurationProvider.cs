using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySynch.Q.Sender.ConfigurationProvider
{
    public class SenderConfigurationProvider:ISenderConfigurationProvider
    {
        public void Load(string fileName)
        {
            
        }

        public bool Save(string fileName)
        {
            throw new NotImplementedException();
        }

        public bool SaveAndRestart(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
