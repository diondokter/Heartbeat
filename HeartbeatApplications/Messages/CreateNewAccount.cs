using ProtoBuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Messages
{
	[ProtoContract]
    public class CreateNewAccountRequest : Request
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
				_Password = Encoding.Unicode.GetString(new Rfc2898DeriveBytes(value, Encoding.Unicode.GetBytes(Username), 2048).GetBytes(512));
			}
		}
	}

	[ProtoContract]
	public class CreateNewAccountResponse : Response
	{
		[ProtoMember(1)]
		public bool Accepted;
		[ProtoMember(2)]
		public string Reason;
	}
}
