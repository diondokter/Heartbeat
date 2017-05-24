using ProtoBuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
	[ProtoContract]
    public class RemoveUserViewPermissionMessage : Message
    {
		[ProtoMember(1)]
		public string Username;
    }
}
