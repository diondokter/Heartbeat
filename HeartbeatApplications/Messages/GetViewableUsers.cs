using ProtoBuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
	[ProtoContract]
    public class GetViewableUsersRequest : Request
    {
		[ProtoMember(1)]
		public int MaxCount;
		[ProtoMember(2)]
		public string WithContaining;
    }

	[ProtoContract]
	public class GetViewableUsersResponse : Response
	{
		[ProtoMember(1)]
		public string[] Usernames;
	}
}
