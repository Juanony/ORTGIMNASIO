using System.ComponentModel.DataAnnotations;

namespace GymMembershipApp.Models
{
    public class MembershipPlan
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public int DurationInDays { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Member> Members { get; set; } = new List<Member>();
    }
}
