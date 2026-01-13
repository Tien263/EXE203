using System.Net;
using System.Net.Mail;

namespace Exe_Demo.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode, string userName)
        {
            var subject = "Mã OTP Xác Thực Tài Khoản - Mộc Vị Store";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #4f6a4c 0%, #6b8f67 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .otp-box {{ background: white; border: 3px dashed #4f6a4c; padding: 20px; text-align: center; margin: 20px 0; border-radius: 10px; }}
                        .otp-code {{ font-size: 36px; font-weight: bold; color: #4f6a4c; letter-spacing: 8px; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🌿 Mộc Vị Store</h1>
                            <p>Xác Thực Tài Khoản</p>
                        </div>
                        <div class='content'>
                            <h2>Xin chào {userName}!</h2>
                            <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>Mộc Vị Store</strong>.</p>
                            <p>Để hoàn tất đăng ký, vui lòng nhập mã OTP bên dưới:</p>
                            
                            <div class='otp-box'>
                                <p style='margin: 0; color: #666;'>Mã OTP của bạn:</p>
                                <div class='otp-code'>{otpCode}</div>
                                <p style='margin: 10px 0 0 0; color: #999; font-size: 14px;'>Mã có hiệu lực trong 5 phút</p>
                            </div>
                            
                            <p><strong>Lưu ý:</strong></p>
                            <ul>
                                <li>Không chia sẻ mã OTP với bất kỳ ai</li>
                                <li>Mã OTP chỉ sử dụng một lần</li>
                                <li>Nếu bạn không yêu cầu đăng ký, vui lòng bỏ qua email này</li>
                            </ul>
                            
                            <p>Trân trọng,<br><strong>Đội ngũ Mộc Vị Store</strong></p>
                        </div>
                        <div class='footer'>
                            <p>© 2025 Mộc Vị Store - Hoa Quả Sấy Mộc Châu</p>
                            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Chào Mừng Đến Với Mộc Vị Store! 🌿";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #4f6a4c 0%, #6b8f67 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🌿 Chào Mừng Đến Với Mộc Vị Store!</h1>
                        </div>
                        <div class='content'>
                            <h2>Xin chào {userName}!</h2>
                            <p>Tài khoản của bạn đã được kích hoạt thành công! 🎉</p>
                            <p>Bạn có thể bắt đầu khám phá và mua sắm các sản phẩm hoa quả sấy chất lượng cao từ Mộc Châu.</p>
                            <p>Chúc bạn có trải nghiệm mua sắm tuyệt vời!</p>
                            <p>Trân trọng,<br><strong>Đội ngũ Mộc Vị Store</strong></p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderConfirmationEmailAsync(Models.Order order)
        {
            var paymentMethodText = order.PaymentMethod == "COD" ? "Thanh toán khi nhận hàng (COD)" : "Chuyển khoản ngân hàng";
            var subject = $"Xác Nhận Đơn Hàng #{order.OrderCode} - Mộc Vị Store";
            
            var productsHtml = "";
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                foreach (var item in order.OrderDetails)
                {
                    productsHtml += $@"
                        <tr>
                            <td style='padding: 10px; border-bottom: 1px solid #eee;'>{item.ProductName}</td>
                            <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: center;'>{item.Quantity}</td>
                            <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: right;'>{item.Price:N0} VNĐ</td>
                            <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: right;'>{item.TotalPrice:N0} VNĐ</td>
                        </tr>";
                }
            }

            var bankInfoHtml = "";
            if (order.PaymentMethod == "Bank")
            {
                bankInfoHtml = $@"
                    <div style='background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                        <h3 style='color: #856404; margin-top: 0;'>💳 Thông Tin Chuyển Khoản</h3>
                        <table style='width: 100%;'>
                            <tr>
                                <td style='padding: 5px 0;'><strong>Ngân hàng:</strong></td>
                                <td style='padding: 5px 0;'>{_configuration["BankTransfer:BankName"]}</td>
                            </tr>
                            <tr>
                                <td style='padding: 5px 0;'><strong>Số tài khoản:</strong></td>
                                <td style='padding: 5px 0; font-size: 18px; color: #4f6a4c;'><strong>{_configuration["BankTransfer:AccountNumber"]}</strong></td>
                            </tr>
                            <tr>
                                <td style='padding: 5px 0;'><strong>Chủ tài khoản:</strong></td>
                                <td style='padding: 5px 0;'>{_configuration["BankTransfer:AccountName"]}</td>
                            </tr>
                            <tr>
                                <td style='padding: 5px 0;'><strong>Số tiền:</strong></td>
                                <td style='padding: 5px 0; font-size: 18px; color: #dc3545;'><strong>{order.FinalAmount:N0} VNĐ</strong></td>
                            </tr>
                            <tr>
                                <td style='padding: 5px 0;'><strong>Nội dung:</strong></td>
                                <td style='padding: 5px 0; font-size: 16px; color: #4f6a4c;'><strong>DH{order.OrderCode}</strong></td>
                            </tr>
                        </table>
                        <p style='margin-bottom: 0; color: #856404;'><strong>⚠️ Lưu ý:</strong> Vui lòng chuyển khoản đúng số tiền và nội dung để đơn hàng được xử lý nhanh chóng.</p>
                    </div>";
            }

            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #4f6a4c 0%, #6b8f67 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; }}
                        .order-box {{ background: white; padding: 20px; margin: 20px 0; border-radius: 10px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; background: #f9f9f9; padding: 20px; border-radius: 0 0 10px 10px; }}
                        table {{ width: 100%; border-collapse: collapse; }}
                        .total-row {{ background: #f8f9fa; font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🌿 Mộc Vị Store</h1>
                            <p>Xác Nhận Đơn Hàng</p>
                        </div>
                        <div class='content'>
                            <div style='text-align: center; margin: 20px 0;'>
                                <div style='display: inline-block; background: #28a745; color: white; padding: 10px 20px; border-radius: 50px;'>
                                    <span style='font-size: 24px;'>✓</span> Đặt Hàng Thành Công!
                                </div>
                            </div>
                            
                            <h2>Xin chào {order.CustomerName}!</h2>
                            <p>Cảm ơn bạn đã đặt hàng tại <strong>Mộc Vị Store</strong>.</p>
                            <p>Đơn hàng của bạn đã được tiếp nhận và đang được xử lý.</p>
                            
                            <div class='order-box'>
                                <h3 style='color: #4f6a4c; margin-top: 0;'>📦 Thông Tin Đơn Hàng</h3>
                                <table style='margin-bottom: 15px;'>
                                    <tr>
                                        <td style='padding: 5px 0;'><strong>Mã đơn hàng:</strong></td>
                                        <td style='padding: 5px 0; color: #4f6a4c;'><strong>#{order.OrderCode}</strong></td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 5px 0;'><strong>Ngày đặt:</strong></td>
                                        <td style='padding: 5px 0;'>{order.CreatedDate:dd/MM/yyyy HH:mm}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 5px 0;'><strong>Người nhận:</strong></td>
                                        <td style='padding: 5px 0;'>{order.CustomerName}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 5px 0;'><strong>Số điện thoại:</strong></td>
                                        <td style='padding: 5px 0;'>{order.CustomerPhone}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 5px 0;'><strong>Địa chỉ:</strong></td>
                                        <td style='padding: 5px 0;'>{order.ShippingAddress}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 5px 0;'><strong>Phương thức thanh toán:</strong></td>
                                        <td style='padding: 5px 0;'>{paymentMethodText}</td>
                                    </tr>
                                </table>
                                
                                <h4 style='color: #4f6a4c;'>Sản phẩm đã đặt:</h4>
                                <table style='width: 100%; border-collapse: collapse;'>
                                    <thead>
                                        <tr style='background: #f8f9fa;'>
                                            <th style='padding: 10px; text-align: left; border-bottom: 2px solid #dee2e6;'>Sản phẩm</th>
                                            <th style='padding: 10px; text-align: center; border-bottom: 2px solid #dee2e6;'>SL</th>
                                            <th style='padding: 10px; text-align: right; border-bottom: 2px solid #dee2e6;'>Đơn giá</th>
                                            <th style='padding: 10px; text-align: right; border-bottom: 2px solid #dee2e6;'>Thành tiền</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {productsHtml}
                                    </tbody>
                                    <tfoot>
                                        <tr class='total-row'>
                                            <td colspan='3' style='padding: 15px; text-align: right; border-top: 2px solid #dee2e6;'>
                                                <strong>Tổng cộng:</strong>
                                            </td>
                                            <td style='padding: 15px; text-align: right; border-top: 2px solid #dee2e6; color: #dc3545; font-size: 18px;'>
                                                <strong>{order.FinalAmount:N0} VNĐ</strong>
                                            </td>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                            
                            {bankInfoHtml}
                            
                            <div style='background: #e7f3ff; border-left: 4px solid #0066cc; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                                <h4 style='color: #0066cc; margin-top: 0;'>📞 Liên Hệ Hỗ Trợ</h4>
                                <p style='margin: 5px 0;'>Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ:</p>
                                <p style='margin: 5px 0;'><strong>Hotline:</strong> 1800-xxxx</p>
                                <p style='margin: 5px 0;'><strong>Email:</strong> {_configuration["EmailSettings:SenderEmail"]}</p>
                            </div>
                            
                            <p>Cảm ơn bạn đã tin tưởng và lựa chọn sản phẩm của chúng tôi!</p>
                            <p>Trân trọng,<br><strong>Đội ngũ Mộc Vị Store</strong></p>
                        </div>
                        <div class='footer'>
                            <p>© 2025 Mộc Vị Store - Hoa Quả Sấy Mộc Châu</p>
                            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(order.CustomerEmail ?? "", subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "your-email@gmail.com";
                var senderPassword = _configuration["EmailSettings:SenderPassword"] ?? "your-app-password";
                var senderName = _configuration["EmailSettings:SenderName"] ?? "Mộc Vị Store";

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, senderPassword)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log error (trong production nên log vào file hoặc database)
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}
