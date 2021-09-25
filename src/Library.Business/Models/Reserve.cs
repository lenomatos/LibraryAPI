namespace Library.Business.Models
{
    public class Reserve : Entity
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public bool Returned { get; set; }

        /* EF Relations */
        public User User { get; set; }
        public Book Book { get; set; }
    }
}
