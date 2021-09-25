using System;

namespace Library.Api.ViewModels
{

    public class UserTokenViewModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
    }
}
