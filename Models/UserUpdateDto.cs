using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace sampleapi.Models
{
    public class UserUpdateDto
    {
        [Required]
        [JsonPropertyName("username")]
        [MaxLength(256)]
        public required string UserName { get; set; }

        [Required]
        [JsonPropertyName("firstname")]
        public required string FirstName { get; set; }

        [Required]
        [JsonPropertyName("lastname")]
        public required string LastName { get; set; }

        [Required]
        [JsonPropertyName("email")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        [JsonPropertyName("phone")]
        [Phone(ErrorMessage = "Invalid phone.")]
        public string Phone { get; set; } = string.Empty;
    }
}
