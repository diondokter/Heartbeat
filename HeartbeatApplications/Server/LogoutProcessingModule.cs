using Messages;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Server
{
	internal class LogoutProcessingModule : GenericMessageProcessingModule<LogoutMessage>
	{
		protected override void Run(LogoutMessage RunTarget, NetworkClient Sender)
		{
			(string Username, NetworkClient Client) UserConnection = User.UserConnections.First(x => x.Client == Sender);

			User.UserConnections.Remove(UserConnection);
			NetworkServer.RemoveClient(Sender);

			Controller.Logger.OnLogReceived($"User {UserConnection.Username} logged out.");
		}
	}
}
