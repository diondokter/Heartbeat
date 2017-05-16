using Messages;
using Protocol;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Client
{
    public static class NetworkManager
    {
		private static NetworkClient _Connection;
		private static NetworkClient Connection
		{
			get
			{
				if (!_Connection?.IsConnected ?? true)
				{
					_Connection = new NetworkClient(IPAddress.Loopback, 5000);
				}

				return _Connection;
			}
		}

		public static async Task<(bool Accepted, string Reason)> Login(string Username, string Password)
		{
			try
			{
				LoginResponse Response = await Connection.Send<LoginResponse>(new LoginRequest() { Username = Username, Password = Password });

				return (Response.Accepted, Response.Reason);
			}
			catch (Exception e)
			{
				return (false, e.Message);
			}
		}

		public static async Task<(bool Accepted, string Reason)> CreateAccount(string Username, string Passsword)
		{
			CreateNewAccountResponse Response = await Connection.Send<CreateNewAccountResponse>(new CreateNewAccountRequest() { Username = Username, Password = Passsword });

			if (Response.Accepted)
			{
				return await Login(Username, Passsword);
			}
			else
			{
				return (Response.Accepted, Response.Reason);
			}
		}

		public static void Logout()
		{
			if (_Connection != null && _Connection.IsConnected)
			{
				Connection.Send(new LogoutMessage());
				Connection.Dispose();
			}
		}

		public static async Task<string[]> GetViewableUsers(int MaxCount, string WithContainingValue)
		{
			return (await Connection.Send<GetViewableUsersResponse>(new GetViewableUsersRequest() { MaxCount = MaxCount, WithContaining = WithContainingValue })).Usernames;
		}
    }
}
