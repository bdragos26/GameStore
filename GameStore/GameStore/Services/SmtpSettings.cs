namespace GameStore.Services
{
    public class SmtpSettings
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; }
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }
}
