using ICinema.Models;
using System.ComponentModel.DataAnnotations;

namespace ICinema.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введіть ім'я")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ''-'-]{1,40}$",
         ErrorMessage = "Недопустимі символи")]
        public string FirstName { get; set; }
       
        
        [Required(ErrorMessage = "Введіть прізвище")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ''-'-]{1,40}$",
         ErrorMessage = "Недопустимі символи")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Невірний формат електронної адреси")]
        [Required(ErrorMessage = "Введіть е-mail")]
        public string Email { get; set; }

        [Phone (ErrorMessage = "Невірний формат номеру телефону")]
        [Required(ErrorMessage = "Введіть номер телефону")]
        [MinLength(10, ErrorMessage = "Номер телефону має складатися з 10 цифр")]
        [MaxLength(10, ErrorMessage = "Номер телефону має складатися з 10 цифр")]
        public string PhoneNumber { get; set; }

        [MinLength(6, ErrorMessage = "Пароль має містити не менше 6 символів")]
        [MaxLength(50, ErrorMessage = "Пароль занадто довгий")]
        [Required(ErrorMessage = "Необхідно ввести пароль")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Повторно введений пароль має співпадати з першим")]
        [Required(ErrorMessage = "Підтвердіть пароль")]
        public string ConfirmPassword { get; set; }

        public User ToRecord()
        {
            User user = new();
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;
            user.PhoneNumber = PhoneNumber;
            user.Password = Password;
            user.UserStatus = "Клієнт";

            return user;
        }
    }
}
