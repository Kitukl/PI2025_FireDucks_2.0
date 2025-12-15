using System.Net.Mail;
using Castle.Core.Smtp;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace StudyHub.BLL.Commands;

public class EmailSender : IEmailSender
{
    private readonly ISendGridClient _client;

    public EmailSender(ISendGridClient client)
    {
        _client = client;
    }

    public async System.Threading.Tasks.Task Send(string to, string subject, string messageText)
    {
        EmailAddress fromEmail = new EmailAddress()
        {
            Email = "kitoleg167@gmail.com",
            Name = "StudyHub Support"
        };
        EmailAddress toEmail = new EmailAddress(to);
        var htmlContent = GenerateHtmlTemplate(messageText);
        var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject,plainTextContent: null, htmlContent);

        var result = await _client.SendEmailAsync(msg);

        if (!result.IsSuccessStatusCode)
        {
            throw new Exception("Email not send!");
        }

    }

    public void Send(string from, string to, string subject, string messageText)
    {
        throw new NotImplementedException();
    }

    public void Send(MailMessage message)
    {
        throw new NotImplementedException();
    }

    public void Send(IEnumerable<MailMessage> messages)
    {
        throw new NotImplementedException();
    }

    private static string GenerateHtmlTemplate(string deadlineInfo)
    {
        return $@"
<div style=""font-family: Arial, sans-serif; background-color: #EAF4FF; padding: 20px; border-radius: 8px; max-width: 600px; margin: 20px auto; border: 1px solid #BFD8FF;"">

    <h1 style=""color: #2B4A6F; text-align: center; margin-bottom: 30px;"">
        Наближається дедлайн
    </h1>

    <p style=""color: #3A3A3A; font-size: 16px; line-height: 1.6;"">
        Шановний користувачу!
    </p>

    <p style=""color: #3A3A3A; font-size: 16px; line-height: 1.6;"">
        Нагадуємо про наступну задачу:
    </p>

    <div style=""background-color: #FFF6E5; border: 1px dashed #E0CFA6; padding: 30px; text-align: center; margin: 30px 0; border-radius: 6px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.05);"">

        <p style=""color: #2B4A6F; font-size: 22px; font-weight: 700; margin: 0;"">
            {deadlineInfo}
        </p>

    </div>

    <p style=""color: #3A3A3A; font-size: 16px; line-height: 1.6;"">
        Будь ласка, переконайтесь, що задача буде виконана вчасно.
    </p>

    <p style=""color: #3A3A3A; font-size: 16px; line-height: 1.6;"">
        Якщо задача вже виконана — ви можете проігнорувати це повідомлення.
    </p>

    <hr style=""border: 0; border-top: 1px solid #BFD8FF; margin: 30px 0;"">

    <p style=""color: #6B6B6B; font-size: 12px; text-align: center;"">
        З повагою,<br>
        Команда StudyHub
    </p>

</div>";
    }
}