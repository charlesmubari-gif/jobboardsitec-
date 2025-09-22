using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobBoard.Models
{
    public class JobApplication
    {
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }
        public Job? Job { get; set; }

        [Required]
        public int ApplicantId { get; set; }
        public User? Applicant { get; set; }

        public string? CoverLetter { get; set; }

        // File path to uploaded CV
        public string? CvFilePath { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
