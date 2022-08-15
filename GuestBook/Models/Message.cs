using System;
using System.Collections.Generic;

namespace GuestBook.Models
{
    public partial class Message
    {
        public Message()
        {
            Users = new HashSet<User>();
        }

        public int MsgId { get; set; }
        public string? Message1 { get; set; }
        public int SenderId { get; set; }
        public int? ReplyMsgId { get; set; }

        public virtual User Sender { get; set; } = null!;

        public virtual ICollection<User> Users { get; set; }
    }
}
