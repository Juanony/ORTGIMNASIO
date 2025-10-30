using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMembershipApp.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        BankTransfer,
        Other
    }

    public class Payment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Display(Name = "Payment Status")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

        [Display(Name = "Transaction Reference")]
        public string? TransactionReference { get; set; }

        public string? Notes { get; set; }

        [Display(Name = "Membership Plan")]
        public int? MembershipPlanId { get; set; }

        // Navigation properties
        [ForeignKey("MemberId")]
        public Member Member { get; set; } = null!;

        [ForeignKey("MembershipPlanId")]
        public MembershipPlan? MembershipPlan { get; set; }
    }
}
