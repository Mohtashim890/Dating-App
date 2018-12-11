using System;

namespace DatingApp.API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; } // foreign key
        public User Sender { get; set; } // sender information...access to user from messages table
        public int RecipientId { get; set; } // foreign key
        public User Recipient { get; set; } // recipient information
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageRead { get; set; } // time of message sent
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }   
}