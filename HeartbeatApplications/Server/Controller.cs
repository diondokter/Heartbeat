using Protocol;
using System;
using System.Linq;

namespace Server
{
    public static class Controller
    {
		public static void Start(int Port, ILogger Logger)
		{
			Message.Initialize();
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
