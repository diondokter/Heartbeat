using ProtoBuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
	[ProtoContract]
	public class GetViewingUsersRequest : Request
	{

	}

	[ProtoContract]
	public class GetViewingUsersResponse : Response
	{
		[ProtoMember(1)]
		public string[] Usernames;
	}
}
