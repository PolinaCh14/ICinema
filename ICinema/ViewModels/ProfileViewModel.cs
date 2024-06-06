using ICinema.Infrastructure.Constants;
using ICinema.Models;
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

        public bool IsEditMode { get; set; }


       public ProfileOrders Orders { get; set; }

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
