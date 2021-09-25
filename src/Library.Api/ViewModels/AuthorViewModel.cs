using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.ViewModels
{
    public class AuthorViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Name { get; set; }
    }
}
