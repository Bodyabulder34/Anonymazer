
using System.ComponentModel.DataAnnotations;

namespace Anonymazer.Models.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Email обязателен.")]
    [EmailAddress(ErrorMessage = "Некорректный формат email.")]
    [StringLength(254, ErrorMessage = "Email слишком длинный.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен.")]
    [StringLength(128, ErrorMessage = "Пароль слишком длинный.")]
    public string Password { get; set; } = string.Empty;
}
