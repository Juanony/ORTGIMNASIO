using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMembershipApp.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required]
        [Display(Name = "Check-In Time")]
        public DateTime CheckInTime { get; set; } = DateTime.Now;

        [Display(Name = "Check-Out Time")]
        public DateTime? CheckOutTime { get; set; }

        public string? Notes { get; set; }

        // Navigation property
        [ForeignKey("MemberId")]
        public Member Member { get; set; } = null!;

        [NotMapped]
        public TimeSpan? Duration => CheckOutTime.HasValue ? CheckOutTime.Value - CheckInTime : null;
    }
}
