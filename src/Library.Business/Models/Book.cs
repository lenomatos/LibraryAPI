using System.ComponentModel.DataAnnotations;

namespace Library.Business.Models
{
    public class Book : Entity
    {
        public string Name { get; set; }
        public int BookGenreId { get; set; }
        public int AuthorId { get; set; }
        public string Synopsis { get; set; }

        /* EF Relations */
        public Author Author { get; set; }
        public BookGenre BookGenre { get; set; }
    }
}
