
using System.ComponentModel.DataAnnotations;

namespace Anonymazer.Models.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Имя пользователя обязательно.")]
    [StringLength(30, MinimumLength = 3,
        ErrorMessage = "Имя пользователя должно содержать от 3 до 30 символов.")]
    [RegularExpression(@"^[\p{L}\p{N}_.-]+$",
        ErrorMessage = "Имя может содержать буквы, цифры, точку, дефис и нижнее подчёркивание.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен.")]
    [EmailAddress(ErrorMessage = "Некорректный формат email.")]
    [StringLength(254, ErrorMessage = "Email слишком длинный.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен.")]
    [StringLength(128, MinimumLength = 8,
        ErrorMessage = "Пароль должен содержать от 8 до 128 символов.")]
    public string Password { get; set; } = string.Empty;
}
