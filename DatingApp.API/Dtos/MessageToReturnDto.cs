using System;

namespace DatingApp.API.Dtos
{
    public class MessageToReturnDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; } // foreign key
        public string SenderKnownAs { get; set; } // not in 'Message.cs' to map but automapper automatically figure it out
        public string SenderPhotoUrl { get; set; } // not in 'Message.cs' to map...automapper failed to figure it out so manually adjust it
        public int RecipientId { get; set; } // foreign key
        public string RecipientKnownAs { get; set; } // not in 'Message.cs' to map but automapper automatically figure it out
        public string RecipientPhotoUrl { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageRead { get; set; } // message sent by user
    }
}