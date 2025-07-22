
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;
using RazorEngine;
using RazorEngine.Templating;

namespace PRN_Final_Project.Service
{
    public class EmailService
    {
        private readonly IUserService _service;
        public EmailService(IUserService service)
        {
            _service = service;
        }

        public async Task SendWelcomeEmailAsync(int userId)
        {
            // --- Phần lấy thông tin user và tạo body vẫn giữ nguyên ---
            var user = await _service.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            string host = Environment.GetEnvironmentVariable("SMTP_HOST");
            int port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));
            string username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            string password = Environment.GetEnvironmentVariable("SMTP_APP_PASSWORD");
            // enableSsl không cần dùng nữa, MailKit sẽ quản lý việc này qua SecureSocketOptions

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Templates", "WelcomeTemplate.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var model = new { FullName = user.first_name + " " + user.last_name, Email = user.email };
            var body = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), null, model);

            // 1. Tạo đối tượng email của MailKit
            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("FIntern", username));
            mail.To.Add(MailboxAddress.Parse(user.email));
            mail.Subject = "Chào mừng bạn đến với FIntern";

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            mail.Body = bodyBuilder.ToMessageBody();

            // 2. Tạo SmtpClient của MailKit và gửi đ
            using (var smtp = new SmtpClient())
            {
                // Kết nối với tùy chọn bảo mật rõ ràng là StartTls
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);

                // Xác thực bằng username và MẬT KHẨU ỨNG DỤNG
                await smtp.AuthenticateAsync(username, password);

                // Gửi email
                await smtp.SendAsync(mail);

                // Ngắt kết nối
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendRejectEmailAsync(int userId)
        {
            // --- Phần lấy thông tin user và tạo body vẫn giữ nguyên ---
            var user = await _service.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            string host = Environment.GetEnvironmentVariable("SMTP_HOST");
            int port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));
            string username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            string password = Environment.GetEnvironmentVariable("SMTP_APP_PASSWORD");
            // enableSsl không cần dùng nữa, MailKit sẽ quản lý việc này qua SecureSocketOptions

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Templates", "RejectTemplate.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var model = new { FullName = user.first_name + " " + user.last_name, Email = user.email };
            var body = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), null, model);

            // 1. Tạo đối tượng email của MailKit
            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("FIntern", username));
            mail.To.Add(MailboxAddress.Parse(user.email));
            mail.Subject = "Thông báo từ FIntern";

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            mail.Body = bodyBuilder.ToMessageBody();

            // 2. Tạo SmtpClient của MailKit và gửi đ
            using (var smtp = new SmtpClient())
            {
                // Kết nối với tùy chọn bảo mật rõ ràng là StartTls
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);

                // Xác thực bằng username và MẬT KHẨU ỨNG DỤNG
                await smtp.AuthenticateAsync(username, password);

                // Gửi email
                await smtp.SendAsync(mail);

                // Ngắt kết nối
                await smtp.DisconnectAsync(true);
            }
        }
    }
}