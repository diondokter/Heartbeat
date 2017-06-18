using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;

namespace UWPClient
{
	public class BaseStationConnection : IDisposable
	{
		private string DeviceID;
		private SerialDevice Device;

		private DataReader InputReader;
		private DataWriter OutputWriter;

		public bool Disposed { get; private set; }

		public BaseStationConnection(string DeviceID)
		{
			this.DeviceID = DeviceID;
		}

		public async Task Initialize()
		{
			if (Disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}

			Device = await SerialDevice.FromIdAsync(DeviceID);

			if (Device == null)
			{
				throw new Exception();
			}

			Device.StopBits = SerialStopBitCount.Two;
			Device.Parity = SerialParity.None;
			Device.DataBits = 8;
			Device.BaudRate = 9600;
			Device.ReadTimeout = TimeSpan.FromSeconds(1);
			Device.WriteTimeout = TimeSpan.FromSeconds(1);

			OutputWriter = new DataWriter(Device.OutputStream) { ByteOrder = ByteOrder.LittleEndian };
			InputReader = new DataReader(Device.InputStream) { ByteOrder = ByteOrder.LittleEndian };
		}

		public ushort GetEntryCount()
		{
			if (Disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}

			OutputWriter.WriteByte(0);
			DataWriterStoreOperation WriteOperation = OutputWriter.StoreAsync();
			WriteOperation.AsTask().Wait();

			DataReaderLoadOperation ReadOperation = InputReader.LoadAsync(2);
			ReadOperation.AsTask().Wait();
			return InputReader.ReadUInt16();
		}

		public (float TimeStamp, float BMPValue) GetFirstEntry()
		{
			if (Disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}

			OutputWriter.WriteByte(1);
			DataWriterStoreOperation WriteOperation = OutputWriter.StoreAsync();
			WriteOperation.AsTask().Wait();

			DataReaderLoadOperation ReadOperation = InputReader.LoadAsync(8);
			ReadOperation.AsTask().Wait();
			return (InputReader.ReadSingle(), InputReader.ReadSingle());
		}

		public float GetCurrentTime()
		{
			if (Disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}

			OutputWriter.WriteByte(2);
			DataWriterStoreOperation WriteOperation = OutputWriter.StoreAsync();
			WriteOperation.AsTask().Wait();

			DataReaderLoadOperation ReadOperation = InputReader.LoadAsync(4);
			ReadOperation.AsTask().Wait();
			return InputReader.ReadSingle();
		}

		public void Dispose()
		{
			Disposed = true;

			OutputWriter.Dispose();
			InputReader.Dispose();
			Device.Dispose();
		}
	}
}
