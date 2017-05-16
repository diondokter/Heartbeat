using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
	public class NetworkClient : IDisposable
	{
		private List<Response> ResponseBuffer = new List<Response>();
		private TcpClient Client;
		private NetworkStream Connection
		{
			get
			{
				if (Disposed)
				{
					return null;
				}

				return Client?.GetStream();
			}
		}

		public IPAddress ConnectedIP
		{
			get
			{
				return ((IPEndPoint)Client?.Client.RemoteEndPoint).Address;
			}
		}

		public bool IsConnected
		{
			get
			{
				return Client.Connected;
			}
		}

		private MessageProcessingModule[] ProcessingModules;

		public NetworkClient(TcpClient Client, params MessageProcessingModule[] ProcessingModules)
		{
			this.Client = Client;
			this.ProcessingModules = ProcessingModules;

			new Task(LoopReceive).Start();
		}

		public NetworkClient(IPAddress ServerIP, int Port, params MessageProcessingModule[] ProcessingModules)
		{
			Client = new TcpClient(AddressFamily.InterNetwork);
			this.ProcessingModules = ProcessingModules;

			Connect(ServerIP, Port);
		}

		private async void Connect(IPAddress ServerIP, int Port)
		{
			try
			{
				await Client.ConnectAsync(ServerIP, Port);
				new Task(LoopReceive).Start();
			}
			catch
			{
				Debug.WriteLine("Couldn't connect");
			}
		}

		public void Send(Message Value)
		{
			lock (Client)
			{
				if (!Client.Connected || Connection == null)
				{
					throw new SocketException((int)SocketError.NotConnected);
				}

				Value.SerializeInto(Connection);
			}
		}

		public async Task<T> Send<T>(Request Value) where T:Response
		{
			Send(Value);

			T Response = null;
			Stopwatch Watch = Stopwatch.StartNew();

			while (Response == null && Watch.ElapsedMilliseconds < 2000)
			{
				lock (ResponseBuffer)
				{
					Response = ResponseBuffer.OfType<T>().FirstOrDefault(x => x.ID == Value.ID);
					if (Response != null)
					{
						ResponseBuffer.Remove(Response);
						return Response;
					}
				}

				await Task.Delay(5);
			}

			return Response;
		}

		private async void LoopReceive()
		{
			while (!Disposed)
			{
				Message ReceivedMessage = Receive();

				if (ReceivedMessage == null)
				{
					await Task.Delay(5);
					continue;
				}

				if (ReceivedMessage is Response)
				{
					lock (ResponseBuffer)
					{
						ResponseBuffer.Add((Response)ReceivedMessage);
					}
				}
				else
				{
					new Task(() =>
					{
						for (int i = 0; i < ProcessingModules.Length; i++)
						{
							ProcessingModules[i].Process(ReceivedMessage, this);
						}
					}).Start();
				}
			}
		}

		private Message Receive()
		{
			lock (Client)
			{
				if (Disposed)
				{
					return null;
				}

				if (Connection == null || !Client.Connected)
				{
					throw new SocketException((int)SocketError.NotConnected);
				}

				while (!Connection.DataAvailable)
				{
					return null;
				}

				return Message.DeserializeFrom(Connection);
			}
		}

		private bool Disposed = false;
		public void Dispose()
		{
			lock (Client)
			{
				Client.Dispose();
				Disposed = true;
			}
		}
	}
}
