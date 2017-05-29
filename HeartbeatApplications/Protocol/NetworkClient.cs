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

				try
				{
					return Client?.GetStream();
				}
				catch (InvalidOperationException)
				{
					Dispose();
					return null;
				}
			}
		}

		private static DelegateMessageProcessingModule<PingRequest> PingProcessingModule = new DelegateMessageProcessingModule<PingRequest>((RunTarget, Sender) => Sender.Send(new PingResponse() { Time = RunTarget.Time }));

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
			await Client.ConnectAsync(ServerIP, Port);
			new Task(LoopReceive).Start();
		}

		public void Send(Message Value)
		{
			lock (Client)
			{
				if (!Client.Connected || !Client.Client.Connected || Connection == null)
				{
					throw new SocketException((int)SocketError.NotConnected);
				}

				Value.SerializeInto(Connection);
			}
		}

		public async Task<T> Send<T>(Request Value, long WaitTime = 2000) where T:Response
		{
			Send(Value);

			T Response = null;
			Stopwatch Watch = Stopwatch.StartNew();

			while (Response == null && Watch.ElapsedMilliseconds < WaitTime)
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
						PingProcessingModule.Process(ReceivedMessage, this);
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
