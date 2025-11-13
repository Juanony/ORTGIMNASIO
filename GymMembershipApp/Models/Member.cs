using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMembershipApp.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Display(Name = "Membership Start Date")]
        [DataType(DataType.Date)]
        public DateTime? MembershipStartDate { get; set; }

        [Display(Name = "Membership End Date")]
        [DataType(DataType.Date)]
        public DateTime? MembershipEndDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public string? Address { get; set; }

        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; }

        [Display(Name = "Emergency Phone")]
        [Phone]
        public string? EmergencyPhone { get; set; }

        public string? Notes { get; set; }

        // Foreign Key
        [Display(Name = "Membership Plan")]
        public int? MembershipPlanId { get; set; }

        // Navigation properties
        [ForeignKey("MembershipPlanId")]
        public MembershipPlan? MembershipPlan { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public bool HasActiveMembership => MembershipEndDate.HasValue && MembershipEndDate.Value >= DateTime.Today;

        [NotMapped]
        public bool HasPaymentDue => MembershipEndDate.HasValue && MembershipEndDate.Value <= DateTime.Today.AddDays(7);
    }
}
