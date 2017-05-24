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

				for (int i = 0; i < 100; i++)
				{
					AddUserData(new UserData()
					{
						Username = Value.Username,
						Time = DateTime.Now.AddHours(-i),
						Value = i
					});
				}
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
			DB.Table<UserViewPermission>().Delete(x => x.Username == Caller && x.ViewedUsername == Username);
			DB.Commit();
		}

		public static List<string> GetViewPermissions(string Caller)
		{
			List<UserViewPermission> Permissions = DB.Table<UserViewPermission>().Where(x => x.Username == Caller).ToList();
			List<string> PermissionNames = Permissions.Select(x => x.ViewedUsername).ToList();
			PermissionNames.Add(Caller);
			return PermissionNames;
		}

		public static string[] GetViewPermissionsContaining(string Caller, string Value, int MaxAmount)
		{
			return GetViewPermissions(Caller).Where(x => x.Contains(Value)).Take(MaxAmount).ToArray();
		}

		public static bool HasViewUserPermission(string Caller, string Username)
		{
			if (Caller == Username)
			{
				return true;
			}

			return DB.Table<UserViewPermission>().Any(x => x.Username == Caller && x.ViewedUsername == Username);
		}

		public static UserData[] GetUserData(string Caller, string TargetUsername)
		{
			if (!HasViewUserPermission(Caller, TargetUsername))
			{
				return null;
			}

			return DB.Table<UserData>().Where(x => x.Username == TargetUsername).ToArray();
		}

		public static UserData[] GetUserData(string Caller, string TargetUsername, DateTime StartDate, DateTime EndDate)
		{
			if (!HasViewUserPermission(Caller, TargetUsername))
			{
				return null;
			}

			DateTime RealEnddate = EndDate.AddDays(1).AddMilliseconds(-1);

			return DB.Table<UserData>().Where(x => x.Username == TargetUsername && x.Time >= StartDate.Date && x.Time <= RealEnddate).ToArray();
		}
	}
}
