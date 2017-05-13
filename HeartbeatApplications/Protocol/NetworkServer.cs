using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocol
{
	public static class NetworkServer
	{
		private static List<NetworkClient> _Clients = new List<NetworkClient>();
		public static ReadOnlyCollection<NetworkClient> Clients
		{
			get
			{
				return _Clients.AsReadOnly();
			}
		}
		private static TcpListener Listener;
		private static CancellationTokenSource ListenerCancelToken;
		private static MessageProcessingModule[] ProcessingModules;

		public static void Start(int Port, params MessageProcessingModule[] ProcessingModules)
		{
			NetworkServer.ProcessingModules = ProcessingModules;

			Listener = new TcpListener(IPAddress.Any, Port);
			Listener.Start();

			ListenerCancelToken = new CancellationTokenSource();
			new Task(Listen, ListenerCancelToken.Token).Start();
		}

		public static void Stop()
		{
			ListenerCancelToken.Cancel();
			Listener?.Stop();
			Listener = null;

			_Clients.ForEach(x => x.Dispose());
		}

		private static async void Listen()
		{
			while (Listener != null)
			{
				_Clients.Add(new NetworkClient(await Listener.AcceptTcpClientAsync(), ProcessingModules));
				Debug.WriteLine("New Connection");
			}
		}

		public static void RemoveClient(NetworkClient Target)
		{
			Target.Dispose();
			_Clients.Remove(Target);
		}
	}
}
