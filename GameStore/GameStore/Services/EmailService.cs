using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GameStore.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string resetToken);
    }

    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }
        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            try
            {
                var resetLink = $"{_settings.BaseUrl}/reset-password?token={resetToken}";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Password Reset - GameStore";

                message.Body = new TextPart("html")
                {
                    Text = $@"
                    <h2>Password Reset Request</h2>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{resetLink}'>{resetLink}</a></p>
                    <p>This link will expire in 24 hours.</p>
                "
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.Server, _settings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Password reset email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email to {Email}", email);
                throw;
            }
        }
    }
}
