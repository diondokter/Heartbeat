using ProtoBuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
	[ProtoContract]
    public class AddUserDataMessage : Message
    {
		[ProtoMember(1)]
		public DateTime Timestamp;
		[ProtoMember(2)]
		public float BPMValue;
    }
}
