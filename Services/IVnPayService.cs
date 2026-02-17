using Exe_Demo.Models.ViewModels;

namespace Exe_Demo.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPayRequestModel model);
        Task<bool> ExecutePayment(IQueryCollection collections);
    }
}
