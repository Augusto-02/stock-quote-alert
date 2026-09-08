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

        message.Subject = $"Alerta de {alertType}: {ticker}";
        message.Body =
            $"Ticker: {ticker}\n" +
            $"Preço atual: {actualPrice}\n" +
            $"Tipo: {alertType}";

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