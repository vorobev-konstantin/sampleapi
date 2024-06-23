using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sampleapi.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        public User(string userName, string firstName, string lastName, string email)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
