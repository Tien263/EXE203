using System.ComponentModel.DataAnnotations;

namespace Exe_Demo.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }
        
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        
        [Display(Name = "Thành phố")]
        public string? City { get; set; }
        
        [Display(Name = "Quận/Huyện")]
        public string? District { get; set; }
        
        [Display(Name = "Phường/Xã")]
        public string? Ward { get; set; }
        
        [Display(Name = "Mã khách hàng")]
        public string? CustomerCode { get; set; }
        
        [Display(Name = "Loại khách hàng")]
        public string? CustomerType { get; set; }
        
        [Display(Name = "Điểm tích lũy")]
        public int? LoyaltyPoints { get; set; }
        
        [Display(Name = "Ngày tham gia")]
        public DateTime? CreatedDate { get; set; }
        
        [Display(Name = "Đăng nhập lần cuối")]
        public DateTime? LastLoginDate { get; set; }
        
        [Display(Name = "Ảnh đại diện")]
        public string? ProfileImageUrl { get; set; }
        
        public IFormFile? ProfileImage { get; set; }
    }
}
