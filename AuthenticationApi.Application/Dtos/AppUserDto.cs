using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.Dtos
{
    public record AppUserDto(
            int id,
            [Required] string UserName,
            [Required] string Phone,
            [Required] string Address,
            [Required, EmailAddress] string Email,
            [Required] string Password,
            [Required] string Role
        );
}
