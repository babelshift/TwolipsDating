using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwolipsDating.Models
{
    public enum MessageStatusValue
    {
        Unread = 1,
        Read = 2,
        Deleted = 3
    }

    public class MessageStatus
    {
        public MessageStatus()
        {
            Messages = new List<Message>();
        }

        public int Id { get; set; }

        [Index("UX_Name", 1, IsUnique = true)]      // EF 6.1 doesn't support first class unique indexes via Fluent API
        public string Name { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}