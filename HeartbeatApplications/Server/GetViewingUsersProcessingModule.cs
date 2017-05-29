using Messages;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
	internal class GetViewingUsersProcessingModule : GenericRequestProcessingModule<GetViewingUsersRequest>
	{
		protected override Response Run(GetViewingUsersRequest RunTarget, NetworkClient Sender)
		{
			string Caller = User.GetUsername(Sender);

			if (Caller == null)
			{
				return null;
			}

			return new GetViewingUsersResponse() { Usernames = DatabaseManager.GetViewingPermissions(Caller) };
		}
	}
}
