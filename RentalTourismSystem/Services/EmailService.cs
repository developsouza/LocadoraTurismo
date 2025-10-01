using Microsoft.Extensions.Options;
using RentalTourismSystem.Models;
using RentalTourismSystem.Models.Configuration;
using System.Net;
using System.Net.Mail;

namespace RentalTourismSystem.Services
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string destinatario, string assunto, string corpo, bool isHtml = true);
        Task EnviarEmailReservaConfirmadaAsync(string emailCliente, string nomeCliente, ReservaViagem reserva);
        Task EnviarEmailCNHVencendoAsync(string emailCliente, string nomeCliente, DateTime validadeCNH);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpo, bool isHtml = true)
        {
            try
            {
                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = _emailSettings.UseSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = assunto,
                    Body = corpo,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(destinatario);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email enviado com sucesso para {Destinatario}", destinatario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email para {Destinatario}", destinatario);
                throw;
            }
        }

        public async Task EnviarEmailReservaConfirmadaAsync(string emailCliente, string nomeCliente, ReservaViagem reserva)
        {
            var assunto = $"Reserva Confirmada - {reserva.PacoteViagem.Nome}";
            var corpo = $@"
                <h2>Olá, {nomeCliente}!</h2>
                <p>Sua reserva foi confirmada com sucesso.</p>
                <h3>Detalhes da Reserva:</h3>
                <ul>
                    <li><strong>Pacote:</strong> {reserva.PacoteViagem.Nome}</li>
                    <li><strong>Destino:</strong> {reserva.PacoteViagem.Destino}</li>
                    <li><strong>Data da Viagem:</strong> {reserva.DataViagem:dd/MM/yyyy}</li>
                    <li><strong>Quantidade de Pessoas:</strong> {reserva.Quantidade}</li>
                    <li><strong>Valor Total:</strong> {reserva.ValorTotal:C}</li>
                </ul>
                <p>Em caso de dúvidas, entre em contato conosco.</p>
                <p>Obrigado por escolher nossos serviços!</p>
            ";

            await EnviarEmailAsync(emailCliente, assunto, corpo);
        }

        public async Task EnviarEmailCNHVencendoAsync(string emailCliente, string nomeCliente, DateTime validadeCNH)
        {
            var diasRestantes = (validadeCNH.Date - DateTime.Now.Date).Days;
            var assunto = "Atenção: CNH próxima do vencimento";
            var corpo = $@"
                <h2>Olá, {nomeCliente}!</h2>
                <p>Identificamos que sua CNH está próxima do vencimento.</p>
                <p><strong>Data de Vencimento:</strong> {validadeCNH:dd/MM/yyyy}</p>
                <p><strong>Dias restantes:</strong> {diasRestantes} dias</p>
                <p>Para continuar utilizando nossos serviços de locação, mantenha sua habilitação sempre atualizada.</p>
                <p>Renove sua CNH o quanto antes para evitar problemas.</p>
            ";

            await EnviarEmailAsync(emailCliente, assunto, corpo);
        }
    }
}
