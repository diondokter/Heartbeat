using ProtoBuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
	[ProtoContract]
	public class AddUserViewPermissionRequest : Request
	{
		[ProtoMember(1)]
		public string Username;
	}

	[ProtoContract]
	public class AddUserViewPermissionResponse : Response
	{
		[ProtoMember(1)]
		public string FailReason;
	}
}
