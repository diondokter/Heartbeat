using System;
using System.Collections.Generic;
using System.Text;
using Protocol;
using Messages;
using System.Linq;

namespace Server
{
	internal class RemoveUserViewPermissionProcessingModule : GenericMessageProcessingModule<RemoveUserViewPermissionMessage>
	{
		protected override void Run(RemoveUserViewPermissionMessage RunTarget, NetworkClient Sender)
		{
			DatabaseManager.RemoveUserViewPermission(User.GetUsername(Sender), RunTarget.Username);
		}
	}
}
