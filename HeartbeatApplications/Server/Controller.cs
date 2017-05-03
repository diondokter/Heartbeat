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
			Message.Initialize(Assembly.Load(new AssemblyName(nameof(Messages))));

			Controller.Logger = Logger;

			NetworkServer.Start(Port,
				new DelegateMessageProcessingModule<Message>((Value, Sender) => Logger?.OnLogReceived($"Received {Value} from {Sender}."))
				);
		}

		public static void Stop()
		{
			NetworkServer.Stop();
		}
	}
}
