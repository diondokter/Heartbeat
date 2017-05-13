using Messages;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
	internal class LoginProcessingModule : GenericRequestProcessingModule<LoginRequest>
	{
		protected override Response Run(LoginRequest RunTarget, NetworkClient Sender)
		{
			User TargetUser = DatabaseManager.GetUser(RunTarget.Username);
			if (TargetUser == null)
			{
				Controller.Logger.OnLogReceived($"Login attempt for {RunTarget.Username}. Not accepted because username is not known.");
				return new LoginResponse() { Accepted = false, Reason = "Unknown username." };
			}

			if (TargetUser.Password != RunTarget.Password)
			{
				Controller.Logger.OnLogReceived($"Login attempt for {RunTarget.Username}. Not accepted because password is wrong.");
				return new LoginResponse() { Accepted = false, Reason = "Wrong password." };
			}

			if (User.UserConnections.Any(x => x.Username == TargetUser.Username))
			{
				Controller.Logger.OnLogReceived($"Login attempt for {RunTarget.Username}. Not accepted because account is already logged in.");
				return new LoginResponse() { Accepted = false, Reason = "User is already logged in." };
			}

			User.UserConnections.Add((RunTarget.Username, Sender));
			Controller.Logger.OnLogReceived($"User logged in as {RunTarget.Username}");
			return new LoginResponse() { Accepted = true };
		}
	}
}
