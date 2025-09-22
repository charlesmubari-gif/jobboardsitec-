using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobBoard.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public string? Location { get; set; }
        public string? Salary { get; set; }
        public string? Category { get; set; }

        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        public DateTime? Deadline { get; set; }

        // employer
        [ForeignKey("Employer")]
        public int EmployerId { get; set; }
        public User? Employer { get; set; }
    }
}
