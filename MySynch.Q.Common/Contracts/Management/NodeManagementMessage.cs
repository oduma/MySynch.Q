namespace MySynch.Q.Common.Contracts.Management
{
    public class NodeManagementMessage
    {
        public object[] cluster_links { get; set; }
        public long disk_free { get; set; }
        public Rate disk_free_details { get; set; }
        public double io_read_avg_time { get; set; }
        public Rate io_read_avg_time_details { get; set; }
        public double io_sync_avg_time { get; set; }
        public Rate io_sync_avg_time_details { get; set; }
        public double io_write_avg_time { get; set; }
        public Rate io_write_avg_time_details { get; set; }
        public long mem_used { get; set; }
        public Rate mem_used_details { get; set; }
        public long mnesia_disk_tx_count { get; set; }
        public Rate mnesia_disk_tx_count_details { get; set; }
        public long mnesia_ram_tx_count { get; set; }
        public Rate mnesia_ram_tx_count_details { get; set; }
        public long proc_used { get; set; }
        public Rate proc_used_details { get; set; }
        public int sockets_used { get; set; }
        public Rate sockets_used_details { get; set; }
        public object[] partitions { get; set; }
        public string os_pid { get; set; }
        public long fd_total { get; set; }
        public int sockets_total { get; set; }
        public long mem_limit { get; set; }
        public bool mem_alarm { get; set; }
        public long disk_free_limit { get; set; }
        public bool disk_free_alarm { get; set; }
        public long proc_total { get; set; }
        public string rates_mode { get; set; }
        public long uptime { get; set; }
        public int run_queue { get; set; }
        public int processors { get; set; }
        public DetailMessage[] exchange_types { get; set; }
        public DetailMessage[] auth_mechanisms { get; set; }
        public VersionMessage[] applications { get; set; }
        public ContextMessage[] contexts { get; set; }
        public string log_file { get; set; }
        public string sasl_log_file { get; set; }
        public string db_dir { get; set; }
        public string[] config_files { get; set; }
        public int net_ticktime { get; set; }
        public string[] enabled_plugins { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public bool running { get; set; }
    }
}
