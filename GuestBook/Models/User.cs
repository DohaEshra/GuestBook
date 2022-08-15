using System;
using System.Collections.Generic;

namespace GuestBook.Models
{
    public partial class User
    {
        public User()
        {
            Messages = new HashSet<Message>();
            Msgs = new HashSet<Message>();
        }

        public int UserId { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<Message> Msgs { get; set; }
    }
}
