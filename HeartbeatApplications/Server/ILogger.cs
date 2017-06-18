using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	public interface ILogger
    {
		void OnLogReceived(string Log);
    }
}
