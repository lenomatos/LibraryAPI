using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Business.Models
{
    public class User : Entity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Administrator { get; set; }

        public IEnumerable<Reserve> Reserves { get; set; }

    }
}
