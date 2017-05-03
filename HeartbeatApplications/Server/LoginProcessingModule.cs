using Messages;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	internal class LoginProcessingModule : GenericMessageProcessingModule<LoginRequest>
	{
		protected override void Run(LoginRequest RunTarget, NetworkClient Sender)
		{
			Controller.Logger.OnLogReceived($"User logged as {RunTarget.Username} {RunTarget.Password} from {Sender.ConnectedIP}");
			Sender.Send(new LoginResponse() { Accepted = true, Reason = "As always" });
		}
	}
}
