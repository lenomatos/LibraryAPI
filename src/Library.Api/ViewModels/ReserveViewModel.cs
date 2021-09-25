using System.ComponentModel.DataAnnotations;

namespace Library.Api.ViewModels
{
    public class ReserveViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int BookId { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public bool Returned { get; set; }

        /* EF Relations */
        public UserViewModel User { get; set; }
        public BookViewModel Book { get; set; }
    }
}
