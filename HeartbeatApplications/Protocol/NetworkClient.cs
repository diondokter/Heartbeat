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

		private MessageProcessingModule[] ProcessingModules;

		public NetworkClient(TcpClient Client, params MessageProcessingModule[] ProcessingModules)
		{
			this.Client = Client;
			this.ProcessingModules = ProcessingModules;

			new Task(async () => await LoopReceive()).Start();
		}

		public NetworkClient(IPAddress ServerIP, int Port, params MessageProcessingModule[] ProcessingModules)
		{
			Client = new TcpClient(AddressFamily.InterNetwork);
			Client.ConnectAsync(ServerIP, Port);
			this.ProcessingModules = ProcessingModules;

			new Task(async () => await LoopReceive()).Start();
		}

		public void Send(Message SendTarget)
		{
			if (Connection == null || !Client.Connected)
			{
				throw new SocketException((int)SocketError.NotConnected);
			}

			SendTarget.SerializeInto(Connection);
		}

		public async Task<T> Send<T>(Request SendTarget) where T:Response
		{
			Send(SendTarget);

			T Response = null;
			Stopwatch Watch = Stopwatch.StartNew();

			while (Response == null && Watch.ElapsedMilliseconds < 2000)
			{
				lock (ResponseBuffer)
				{
					Response = ResponseBuffer.OfType<T>().FirstOrDefault(x => x.ID == SendTarget.ID);
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

		private async Task LoopReceive()
		{
			while (!Disposed)
			{
				Message ReceivedMessage = await Receive();

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

		private async Task<Message> Receive()
		{
			if (Connection == null || !Client.Connected)
			{
				throw new SocketException((int)SocketError.NotConnected);
			}

			while (!Connection.DataAvailable)
			{
				await Task.Delay(5);
			}

			return Message.DeserializeFrom(Connection);
		}

		private bool Disposed = false;
		public void Dispose()
		{
			Client.Dispose();
			Disposed = true;
		}
	}
}
