using Messages;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
	internal class GetViewableUsersProcessingModule : GenericRequestProcessingModule<GetViewableUsersRequest>
	{
		protected override Response Run(GetViewableUsersRequest RunTarget, NetworkClient Sender)
		{
			string Caller = User.UserConnections.FirstOrDefault(x => x.Client == Sender).Username;

			if (Caller == null)
			{
				return null;
			}

			GetViewableUsersResponse Response = new GetViewableUsersResponse();
			Response.Usernames = DatabaseManager.GetViewPermissionsContaining(Caller, RunTarget.WithContaining, RunTarget.MaxCount);

			return Response;
		}
	}
}
