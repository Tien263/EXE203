using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exe_Demo.Models
{
    public class OtpVerification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6)]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddMinutes(5);

        public bool IsUsed { get; set; } = false;

        public bool IsExpired => DateTime.Now > ExpiresAt;
    }
}
