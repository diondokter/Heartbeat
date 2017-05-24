using Protocol;
using System;
using System.Linq;
using System.Reflection;

namespace Server
{
    public static class Controller
    {
		public static ILogger Logger;

		public static void Start(int Port, ILogger Logger)
		{
			Controller.Logger = Logger;

			Message.Initialize(Assembly.Load(new AssemblyName(nameof(Messages))));
			DatabaseManager.Initialize();

			NetworkServer.Start(Port,
				new DelegateMessageProcessingModule<Message>((Value, Sender) => Logger?.OnLogReceived($"Received {Value} from {Sender.ConnectedIP}.")),
				new LoginProcessingModule(),
				new CreateAccountProcessingModule(),
				new LogoutProcessingModule(),
				new GetViewableUsersProcessingModule(),
				new GetUserDataProcessingModule(),
				new AddUserViewPermissionProcessingModule(),
				new RemoveUserViewPermissionProcessingModule()
				);
		}

		public static void Stop()
		{
			NetworkServer.Stop();
		}
	}
}
