using ICinema.Infrastructure.Constants;
using ICinema.Models;
using ICinema.ViewModels.HelperModels;
using System.ComponentModel.DataAnnotations;

namespace ICinema.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Введіть ім'я")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ''-'-]{1,40}$",
       ErrorMessage = "Недопустимі символи")]
        public string FirstName { get; set; } = null!;


        [Required(ErrorMessage = "Введіть прізвище")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ''-'-]{1,40}$",
         ErrorMessage = "Недопустимі символи")]
        public string LastName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Невірний формат електронної адреси")]
        [Required(ErrorMessage = "Введіть е-mail")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Невірний формат номеру телефону")]
        [MinLength(10, ErrorMessage = "Номер телефону має складатися з 10 цифр")]
        [MaxLength(10, ErrorMessage = "Номер телефону має складатися з 10 цифр")]
        public string? PhoneNumber { get; set; } = null;


        [MinLength(6, ErrorMessage = "Пароль має містити не менше 6 символів")]
        [MaxLength(50, ErrorMessage = "Пароль занадто довгий")]
        public string? Password { get; set; } = null!;

        public bool IsEditMode { get; set; }

        public List<OrdersModel> Orders { get; set; } = default!;

        public ProfileViewModel() { }
        public ProfileViewModel(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
        }
    }
}
