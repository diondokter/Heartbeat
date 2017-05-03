using ProtoBuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Messages
{
	[ProtoContract]
    public class LoginRequest : Request
	{
		[ProtoMember(1)]
		public string Username;

		[ProtoMember(2)]
		private string _Password;
		public string Password
		{
			get
			{
				return _Password;
			}
			set
			{
				_Password = Encoding.UTF8.GetString(new Rfc2898DeriveBytes(value, Encoding.UTF32.GetBytes(Username), 2048).GetBytes(512));
			}
		}
    }

	[ProtoContract]
	public class LoginResponse : Response
	{
		[ProtoMember(1)]
		public bool Accepted;
		[ProtoMember(2)]
		public string Reason;
	}
}
