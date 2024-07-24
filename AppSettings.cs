namespace AssetTrack.Core
{
    public class AppSettings
    {
        public BrokerConfiguration BrokerConfiguration { get; set; } = null!;
    }
    public class BrokerConfiguration
    {
        public required string Host { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Port { get; set; }
    }
}
