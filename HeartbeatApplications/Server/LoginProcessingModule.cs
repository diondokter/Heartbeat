using Messages;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			(string Username, NetworkClient Client) ConnectedUser = User.UserConnections.FirstOrDefault(x => x.Username == TargetUser.Username);
			if (ConnectedUser.Client != null)
			{
				Task<PingResponse> SendTask = ConnectedUser.Client.Send<PingResponse>(new PingRequest(), 1000);
				try
				{
					SendTask.Wait();
				}
				catch (AggregateException)
				{
					SendTask = null;
				}

				if (SendTask?.Result != null)
				{
					Controller.Logger.OnLogReceived($"Login attempt for {RunTarget.Username}. Not accepted because account is already logged in.");
					return new LoginResponse() { Accepted = false, Reason = "User is already logged in." };
				}
				else
				{
					Controller.Logger.OnLogReceived($"Login attempt for {RunTarget.Username}. There was still a connection to it, but it did not respond. The connection has been severed in favor of the new login.");
					ConnectedUser.Client.Dispose();
					User.UserConnections.Remove(ConnectedUser);
				}
			}

			User.UserConnections.Add((RunTarget.Username, Sender));
			Controller.Logger.OnLogReceived($"User logged in as {RunTarget.Username}");
			return new LoginResponse() { Accepted = true };
		}
	}
}
