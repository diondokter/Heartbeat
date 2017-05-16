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

		public static IEnumerable<string> GetViewPermissions(string Caller)
		{
			return DB.Table<UserViewPermission>().Where(x => x.Username == Caller).Select(x => x.ViewedUsername).Concat(new string[] { Caller });
		}

		public static IEnumerable<string> GetViewPermissionsContaining(string Caller, string Value, int MaxAmount)
		{
			return GetViewPermissions(Caller).Where(x => x.Contains(Value)).Take(MaxAmount);
		}

		public static bool HasViewUserPermission(string Caller, string Username)
		{
			if (Caller == Username)
			{
				return true;
			}

			return DB.Table<UserViewPermission>().Any(x => x.Username == Caller && x.ViewedUsername == Username);
		}

		public static UserData[] GetUserData(string Caller, string Username)
		{
			if (!HasViewUserPermission(Caller, Username))
			{
				return null;
			}

			return DB.Table<UserData>().Where(x => x.Username == Username).ToArray();
		}

		public static UserData[] GetUserData(string Caller, string Username, DateTime LowerTime, DateTime UpperTime)
		{
			if (!HasViewUserPermission(Caller, Username))
			{
				return null;
			}

			return DB.Table<UserData>().Where(x => x.Username == Username && x.Time >= LowerTime && x.Time <= UpperTime).ToArray();
		}
	}
}
