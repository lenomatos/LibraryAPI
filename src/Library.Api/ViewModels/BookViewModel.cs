using System.ComponentModel.DataAnnotations;

namespace Library.Api.ViewModels
{
    public class BookViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int BookGenreId { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int AuthorId { get; set; }
        public string Synopsis { get; set; }

        public AuthorViewModel Author { get; set; }
        public BookGenreViewModel BookGenre { get; set; }
    }
}
