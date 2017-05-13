using Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Messages;

namespace Server
{
	internal class CreateAccountProcessingModule : GenericRequestProcessingModule<CreateNewAccountRequest>
	{
		protected override Response Run(CreateNewAccountRequest RunTarget, NetworkClient Sender)
		{
			if (DatabaseManager.AddUser(new User() { Username = RunTarget.Username, Password = RunTarget.Password }))
			{
				Controller.Logger.OnLogReceived($"New user added: {RunTarget.Username}");
				return new CreateNewAccountResponse() { Accepted = true };
			}
			else
			{
				Controller.Logger.OnLogReceived($"Tried to make new account: {RunTarget.Username} but it already exists.");
				return new CreateNewAccountResponse() { Accepted = false, Reason = "Username already exists." };
			}
		}
	}
}
