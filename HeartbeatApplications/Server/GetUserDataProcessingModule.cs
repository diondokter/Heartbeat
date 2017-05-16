using Messages;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
	internal class GetUserDataProcessingModule : GenericRequestProcessingModule<GetUserDataRequest>
	{
		protected override Response Run(GetUserDataRequest RunTarget, NetworkClient Sender)
		{
			UserData[] Data = DatabaseManager.GetUserData(User.UserConnections.First(x => x.Client == Sender).Username, RunTarget.TargetUsername, RunTarget.StartDate, RunTarget.EndDate);

			Controller.Logger.OnLogReceived($"Sending {RunTarget.TargetUsername}'s {Data.Length} data values from {RunTarget.StartDate.Date} to {RunTarget.EndDate.Date}.");

			return new GetUserDataResponse() { Data = Data };
		}
	}
}
