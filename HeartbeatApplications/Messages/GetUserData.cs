using ProtoBuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
	[ProtoContract]
    public class GetUserDataRequest : Request
    {
		[ProtoMember(1)]
		public string TargetUsername;
		[ProtoMember(2)]
		public DateTime StartDate;
		[ProtoMember(3)]
		public DateTime EndDate;
	}

	[ProtoContract]
	public class GetUserDataResponse : Response
	{
		[ProtoMember(1, IsRequired = true)]
		public UserData[] Data;
	}
}
