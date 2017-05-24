using System;
using System.Collections.Generic;
using System.Text;
using Protocol;
using Messages;
using System.Linq;

namespace Server
{
	internal class AddUserViewPermissionProcessingModule : GenericRequestProcessingModule<AddUserViewPermissionRequest>
	{
		protected override Response Run(AddUserViewPermissionRequest RunTarget, NetworkClient Sender)
		{
			string FailReason = DatabaseManager.AddUserViewPermission(User.GetUsername(Sender), RunTarget.Username);
			return new AddUserViewPermissionResponse() { FailReason = FailReason };
		}
	}
}
