using System.ComponentModel.DataAnnotations;

namespace JobBoard.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string DisplayName { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public Role Role { get; set; } = Role.Applicant;
    }
}
