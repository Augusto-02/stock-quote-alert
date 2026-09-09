namespace stock_quote_alert;

using stock_quote_alert.Config;
using System.Net;
using System.Net.Mail;

public class EmailAlertService : IAlertService
{
    private readonly SmtpSettings _settings;

    public EmailAlertService(SmtpSettings settings)
    {
        _settings = settings;
    }

    public async Task SendAlertAsync(
        string ticker,
        decimal actualPrice,
        string alertType)
    {
        using MailMessage message = new(
            _settings.From,
            _settings.To);

        message.Subject =
            $"Stock Price Alert: {ticker} - {alertType}";

        message.IsBodyHtml = true;

        message.Body = $"""
                        <html>
                            <body>
                                <h2>Stock Price Alert</h2>

                                <p>
                                    A price condition was reached for
                                    <strong>{ticker}</strong>.
                                </p>

                                <table>
                                    <tr>
                                        <td><strong>Alert type:</strong></td>
                                        <td>{alertType}</td>
                                    </tr>
                                    <tr>
                                        <td><strong>Current price:</strong></td>
                                        <td>R$ {actualPrice:F2}</td>
                                    </tr>
                                </table>

                                <p>
                                    Please check your investment account for more details.
                                </p>
                            </body>
                        </html>
                        """;

        using SmtpClient smtpClient = new(
            _settings.Host,
            _settings.Port);

        smtpClient.EnableSsl = true;
        smtpClient.Credentials = new NetworkCredential(
            _settings.Username,
            _settings.Password);

        await smtpClient.SendMailAsync(message);
    }
}