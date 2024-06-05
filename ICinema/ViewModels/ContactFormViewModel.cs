using System.ComponentModel.DataAnnotations;

namespace ICinema.ViewModels
{
    public class ContactFormViewModel
    {
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Введіть ім'я")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ''-'-]{1,40}$", ErrorMessage = "Недопустимі символи")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Введіть прізвище")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ''-'-]{1,40}$", ErrorMessage = "Недопустимі символи")]
        public string LastName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Невірний формат електронної адреси")]
        [Required(ErrorMessage = "Введіть е-mail")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Оберіть тип оплати")]
        public string PaymentType { get; set; } = null!;

        public string? PaymentCredentials { get; set; }
    }
}
