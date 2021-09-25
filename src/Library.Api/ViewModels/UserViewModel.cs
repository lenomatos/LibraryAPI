using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.ViewModels
{
    public class UserViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(32, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public bool Administrator { get; set; }

        public IEnumerable<ReserveViewModel> Reserves { get; set; }
    }
}
