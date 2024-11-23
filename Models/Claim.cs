using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ClaimsSystem.Models
{

    //get,set methods
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }
        public string User { get; set; } = string.Empty;
        [Range(1, 7)]
        public int HoursWorked { get; set; } = 0;
        public int HourlyRate { get; set; } = 0;
        public int TotalPayment { get; set; } = 0;
        [NotMapped]
        public IFormFile? File { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
    }
}
