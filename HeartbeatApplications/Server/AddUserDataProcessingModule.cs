using Messages;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	public class AddUserDataProcessingModule : GenericMessageProcessingModule<AddUserDataMessage>
	{
		protected override void Run(AddUserDataMessage RunTarget, NetworkClient Sender)
		{
			DatabaseManager.AddUserData(new UserData() { Username = User.GetUsername(Sender), Time = RunTarget.Timestamp, Value = RunTarget.BPMValue });
		}
	}
}
