using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwolipsDating.Utilities
{
    public class ConversationKey
    {
        public string ParticipantUserId1 { get; set; }
        public string ParticipantUserId2 { get; set; }

        public override bool Equals(object obj)
        {
            var conversation = obj as ConversationKey;
            if (conversation != null)
            {
                if (ParticipantUserId1 == conversation.ParticipantUserId1 && ParticipantUserId2 == conversation.ParticipantUserId2
                    || ParticipantUserId1 == conversation.ParticipantUserId2 && ParticipantUserId2 == conversation.ParticipantUserId1)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = ParticipantUserId1.GetHashCode() + ParticipantUserId2.GetHashCode();
            return hash;
        }
    }
}