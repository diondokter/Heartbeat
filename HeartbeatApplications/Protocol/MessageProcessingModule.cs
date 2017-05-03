using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
	public abstract class MessageProcessingModule
	{
		public abstract Type AcceptedType { get; }
		public void Process(Message ProcessTarget, NetworkClient Sender)
		{
			if (ProcessTarget.GetType() == AcceptedType)
			{
				Run(ProcessTarget, Sender);
			}
		}

		protected abstract void Run(Message ReceivedMessage, NetworkClient Sender);
	}

	public abstract class GenericMessageProcessingModule<T> : MessageProcessingModule where T : Message
	{
		public override Type AcceptedType
		{
			get
			{
				return typeof(T);
			}
		}

		protected override void Run(Message RunTarget, NetworkClient Sender)
		{
			Run((T)RunTarget, Sender);
		}

		protected abstract void Run(T RunTarget, NetworkClient Sender);
	}

	public class DelegateMessageProcessingModule<T> : GenericMessageProcessingModule<T> where T : Message
	{
		private Action<T,NetworkClient> Processor;

		public DelegateMessageProcessingModule(Action<T,NetworkClient> Processor)
		{
			this.Processor = Processor;
		}

		protected override void Run(T RunTarget, NetworkClient Sender)
		{
			Processor(RunTarget, Sender);
		}
	}
}
