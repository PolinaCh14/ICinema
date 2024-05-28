using ICinema.Models;
using System.ComponentModel.DataAnnotations;

namespace ICinema.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введіть е-mail або номер телефону")]
        public string EmailOrPhone { get; set; } = null!;


        [Required(ErrorMessage = "Необхідно ввести пароль")]
        public string Password { get; set; } = null!;
    }
}
