using Messages;
using Protocol;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Client
{
    public static class NetworkManager
    {
		public static string CurrentUsername { get; private set; }

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

				if (Response.Accepted)
				{
					CurrentUsername = Username;
				}
				else
				{
					_Connection.Dispose();
					_Connection = null;
				}

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
			GetViewableUsersResponse Response = await Connection.Send<GetViewableUsersResponse>(new GetViewableUsersRequest() { MaxCount = MaxCount, WithContaining = WithContainingValue });
			return Response.Usernames ?? new string[0];
		}

		public static async Task<string[]> GetViewingUsers()
		{
			GetViewingUsersResponse Response = await Connection.Send<GetViewingUsersResponse>(new GetViewingUsersRequest());
			return Response.Usernames ?? new string[0];
		}

		public static async Task<UserData[]> GetUserData(string TargetUsername, DateTime StartDate, DateTime EndDate)
		{
			GetUserDataResponse Response = await Connection.Send<GetUserDataResponse>(new GetUserDataRequest() { TargetUsername = TargetUsername, StartDate = StartDate, EndDate = EndDate });

			return Response?.Data ?? new UserData[0];
		}

		public static async Task<string> AddUserViewPermission(string Username)
		{
			if (Username == CurrentUsername)
			{
				return "You can't add yourself.";
			}

			AddUserViewPermissionResponse Response = await Connection.Send<AddUserViewPermissionResponse>(new AddUserViewPermissionRequest() { Username = Username });
			return Response.FailReason;
		}

		public static void RemoveUserViewPermission(string Username)
		{
			Connection.Send(new RemoveUserViewPermissionMessage() { Username = Username });
		}
	}
}
