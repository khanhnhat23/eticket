using System.ComponentModel.DataAnnotations;
namespace eTickets.Models
{
    public class EmailMarketingModel
    {
        [Required(ErrorMessage ="Vui Lòng Nhập Email")]
        [EmailAddress(ErrorMessage ="Email không Hợp lệ ")]

        public string Email { get; set; }

        [Required(ErrorMessage ="Vui Lòng Nhập Tên")]
        public string Name {  get; set; }
    }
}
