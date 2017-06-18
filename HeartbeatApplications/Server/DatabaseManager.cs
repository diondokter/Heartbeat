using Messages;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public static class DatabaseManager
    {
		private const int MaxDataLength = 200;
		private static SQLiteConnection DB;

		public static void Initialize()
		{
			DB = new SQLiteConnection("Database.db");
			DB.CreateTable<User>();
			DB.CreateTable<UserViewPermission>();
			DB.CreateTable<UserData>();

			Controller.Logger.OnLogReceived($"Database is initialized. Path = {DB.DatabasePath}");
		}

		public static User GetUser(string Username)
		{
			return DB.Table<User>().FirstOrDefault(x => x.Username == Username);
		}

		public static bool AddUser(User Value)
		{
			if (DB.Table<User>().FirstOrDefault(x => x.Username == Value.Username) == null)
			{
				DB.Insert(Value);
				DB.Commit();
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void AddUserData(UserData Value)
		{
			DB.Insert(Value);
			DB.Commit();
		}

		public static void AddUserData(UserData[] Values)
		{
			for (int i = 0; i < Values.Length; i++)
			{
				DB.Insert(Values[i]);
			}
			DB.Commit();
		}

		public static string AddUserViewPermission(string Caller, string Username)
		{
			bool AlreadyExists = DB.Table<UserViewPermission>().Any(x => x.Username == Username && x.ViewedUsername == Caller);

			if (!AlreadyExists)
			{
				bool UserExists = DB.Table<User>().Any(x => x.Username == Username);

				if (UserExists)
				{
					DB.Insert(new UserViewPermission() { Username = Username, ViewedUsername = Caller });
					DB.Commit();

					return null;
				}
				else
				{
					return $"{Username} is not recognized.";
				}
			}
			else
			{
				return $"{Username} already has viewing permission for {Caller}.";
			}
		}

		public static void RemoveUserViewPermission(string Caller, string Username)
		{
			DB.Table<UserViewPermission>().Delete(x => x.Username == Username && x.ViewedUsername == Caller);
			DB.Commit();
		}

		public static List<string> GetViewablePermissions(string Caller)
		{
			List<UserViewPermission> Permissions = DB.Table<UserViewPermission>().Where(x => x.Username == Caller).ToList();
			List<string> PermissionNames = Permissions.Select(x => x.ViewedUsername).ToList();
			PermissionNames.Add(Caller);
			return PermissionNames;
		}

		public static string[] GetViewablePermissionsContaining(string Caller, string Value, int MaxAmount)
		{
			return GetViewablePermissions(Caller).Where(x => x.Contains(Value)).Take(MaxAmount).ToArray();
		}

		public static string[] GetViewingPermissions(string Caller)
		{
			List<UserViewPermission> Permissions = DB.Table<UserViewPermission>().Where(x => x.ViewedUsername == Caller).ToList();
			return Permissions.Select(x => x.Username).ToArray();
		}

		public static bool HasViewUserPermission(string Caller, string Username)
		{
			if (Caller == Username)
			{
				return true;
			}

			return DB.Table<UserViewPermission>().Any(x => x.Username == Caller && x.ViewedUsername == Username);
		}

		public static UserData[] GetUserData(string Caller, string TargetUsername, DateTime StartDate, DateTime EndDate)
		{
			if (!HasViewUserPermission(Caller, TargetUsername))
			{
				return null;
			}

			UserData[] TableData = DB.Table<UserData>().Where(x => x.Username == TargetUsername && x.Time >= StartDate.Date && x.Time <= EndDate).ToArray();

			TimeSpan TotalTime = EndDate - StartDate;
			TimeSpan DeltaTime = TimeSpan.FromMinutes(TotalTime.TotalMinutes / (MaxDataLength - 1));

			UserData[] ReducedData = new UserData[MaxDataLength];

			for (int i = 0; i < MaxDataLength; i++)
			{
				IEnumerable<UserData> PeriodData = TableData.Where(x => x.Time > StartDate + TimeSpan.FromMinutes(DeltaTime.TotalMinutes * i) && x.Time < StartDate + TimeSpan.FromMinutes(DeltaTime.TotalMinutes * (i + 1)));

				UserData CombinedData = new UserData();
				CombinedData.Time = StartDate + TimeSpan.FromMinutes(DeltaTime.TotalMinutes * i);

				if (PeriodData.Any())
				{
					CombinedData.Username = PeriodData.First().Username;
					CombinedData.Value = PeriodData.Average(x => x.Value);
				}

				ReducedData[i] = CombinedData;
			}

			return ReducedData;
		}
	}
}
