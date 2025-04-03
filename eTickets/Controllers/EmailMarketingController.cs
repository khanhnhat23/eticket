using eTickets.Models;
using eTickets.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

public class EmailMarketingController : Controller
{
    private readonly EmailService _emailService;

    public EmailMarketingController(EmailService emailService)
    {
        _emailService = emailService;
    }

    // Hiển thị form nhập email + họ tên
    public IActionResult SendMarketingEmail()
    {
        return View();
    }

    // Nhận dữ liệu từ form và gửi email
    [HttpPost]
    public async Task<IActionResult> SendMarketingEmail(EmailMarketingModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Nếu có lỗi, trả về form với thông báo lỗi
        }

        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Templates/EmailTemplate.html");
        string link = "https://your-marketing-link.com"; // Link khuyến mãi

        string emailContent = await _emailService.LoadEmailTemplateAsync(templatePath, model.Name, link);
        await _emailService.SendEmailAsync(model.Email, "Ưu đãi đặc biệt cho bạn!", emailContent);

        ViewBag.Message = "Email đã được gửi thành công!";
        return View();
    }
}
