using System.IO;
using System.Linq;
using MySynch.Q.Common.Contracts.Management;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MySynch.Q.Tests
{
    [TestFixture]
    public class SerializationContractsTests
    {
        [Test]
        public void DeserializeOk()
        {
            var message = File.ReadAllText("example.json");
            var messageObject = JsonConvert.DeserializeObject<NodeManagementMessage>(message);
            Assert.IsNotNull(messageObject);
            Assert.IsEmpty(messageObject.cluster_links);
            Assert.AreEqual(27395186688, messageObject.disk_free);
            Assert.AreEqual(messageObject.disk_free_details.rate, 22937.6);
            Assert.AreEqual(messageObject.io_read_avg_time, 0);
            Assert.AreEqual(messageObject.io_read_avg_time_details.rate, 0.0);
            Assert.AreEqual(messageObject.io_sync_avg_time, 0);
            Assert.AreEqual(messageObject.io_sync_avg_time_details.rate, 0.0);
            Assert.AreEqual(messageObject.io_write_avg_time, 0);
            Assert.AreEqual(messageObject.io_write_avg_time_details.rate, 0.0);
            Assert.AreEqual(messageObject.mem_used, 40467216);
            Assert.AreEqual(messageObject.mem_used_details.rate, -27888.0);
            Assert.AreEqual(messageObject.mnesia_disk_tx_count, 37);
            Assert.AreEqual(messageObject.mnesia_disk_tx_count_details.rate, 0.0);
            Assert.AreEqual(messageObject.mnesia_ram_tx_count, 44);
            Assert.AreEqual(messageObject.mnesia_ram_tx_count_details.rate, 0.0);
            Assert.AreEqual(messageObject.proc_used, 204);
            Assert.AreEqual(messageObject.proc_used_details.rate, -1.0);
            Assert.AreEqual(messageObject.sockets_used, 3);
            Assert.AreEqual(messageObject.sockets_used_details.rate, 0.0);
            Assert.IsEmpty(messageObject.partitions);
            Assert.AreEqual(messageObject.os_pid, "19592");
            Assert.AreEqual(messageObject.fd_total, 8192);
            Assert.AreEqual(messageObject.sockets_total, 7280);
            Assert.AreEqual(messageObject.mem_limit, 6832291840);
            Assert.False(messageObject.mem_alarm);
            Assert.AreEqual(messageObject.disk_free_limit, 50000000);
            Assert.False(messageObject.disk_free_alarm);
            Assert.AreEqual(messageObject.proc_total, 1048576);
            Assert.AreEqual(messageObject.rates_mode, "basic");
            Assert.AreEqual(messageObject.uptime, 1180327642);
            Assert.AreEqual(messageObject.run_queue, 0);
            Assert.AreEqual(messageObject.processors, 4);
            ValidateExchangeTypes(messageObject.exchange_types);
            ValidateAuthMechanisms(messageObject.auth_mechanisms);
            ValidateApplications(messageObject.applications);
            ValidateContexts(messageObject.contexts);
            Assert.AreEqual(messageObject.log_file,
                @"C:/Users/oduma/AppData/Roaming/RabbitMQ/log/rabbit@EUNBH6SMTZ1.log");
            Assert.AreEqual(messageObject.sasl_log_file,
                @"C:/Users/oduma/AppData/Roaming/RabbitMQ/log/rabbit@EUNBH6SMTZ1-sasl.log");
            Assert.AreEqual(messageObject.db_dir,
                @"c:/Users/oduma/AppData/Roaming/RabbitMQ/db/rabbit@EUNBH6SMTZ1-mnesia");
            Assert.IsNotEmpty(messageObject.config_files);
            Assert.AreEqual(messageObject.config_files[0],
                @"c:/Users/oduma/AppData/Roaming/RabbitMQ/rabbitmq.config (not found)");
            Assert.AreEqual(messageObject.net_ticktime, 60);
            Assert.IsNotEmpty(messageObject.enabled_plugins);
            Assert.AreEqual(messageObject.enabled_plugins[0], "rabbitmq_management");
            Assert.AreEqual(messageObject.name, "rabbit@EUNBH6SMTZ1");
            Assert.AreEqual(messageObject.type, "disc");
            Assert.True(messageObject.running);
        }

        private void ValidateContexts(ContextMessage[] contexts)
        {
            Assert.IsNotEmpty(contexts);
            Assert.AreEqual(contexts[0].description,"RabbitMQ Management");
            Assert.AreEqual(contexts[0].path,"/");
            Assert.AreEqual(contexts[0].port, "15672");
        }

        private void ValidateApplications(VersionMessage[] applications)
        {
            Assert.IsNotEmpty(applications);
            Assert.AreEqual(applications.Length,applications.Count(a=>!string.IsNullOrEmpty(a.version)));
        }

        private void ValidateAuthMechanisms(DetailMessage[] authMechanisms)
        {
            Assert.IsNotEmpty(authMechanisms);
            Assert.AreEqual(3,authMechanisms.Length);
            Assert.AreEqual(1,authMechanisms.Count(a=>!a.enabled));
        }

        private void ValidateExchangeTypes(DetailMessage[] exchangeTypes)
        {
            Assert.IsNotEmpty(exchangeTypes);
            Assert.True(exchangeTypes.All(e=>e.enabled));
            Assert.AreEqual(1,exchangeTypes.Count(e=>e.name=="topic"));
        }
    
    }
}
