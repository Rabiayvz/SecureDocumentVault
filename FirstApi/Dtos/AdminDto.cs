using System.ComponentModel.DataAnnotations;

namespace FirstApi.Dtos
{
    public class AssignRoleDto
    {
        [Required] 
        [Range(1, 4, ErrorMessage = "RoleId must be between 1 and 4.")]
        public int RoleId { get; set; }
    }

    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}