using Protocol;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	public class User
    {
		public static HashSet<(string Username, NetworkClient Client)> UserConnections { get; } = new HashSet<(string, NetworkClient)>();

		[PrimaryKey]
		[MaxLength(64)]
		public string Username { get; set; }
		[MaxLength(512)]
		public string Password { get; set; }
	}

	public class UserViewPermission
	{
		[MaxLength(64)]
		public string Username { get; set; }
		[MaxLength(64)]
		public string ViewedUsername { get; set; }
	}
}
