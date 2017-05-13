using SQLite;
using System;
using System.Collections.Generic;
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
    }
}
