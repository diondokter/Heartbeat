using ProtoBuf;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
	[ProtoContract]
	public struct UserData
	{
		[MaxLength(64)]
		[ProtoMember(1)]
		public string Username { get; set; }
		[ProtoMember(2)]
		public DateTime Time { get; set; }
		[ProtoMember(3)]
		public float Value { get; set; }
	}
}
