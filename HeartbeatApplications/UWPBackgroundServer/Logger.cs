using Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPBackgroundServer
{
	internal class Logger : ILogger
	{
		public void OnLogReceived(string Log)
		{
			Debug.WriteLine(Log);
		}
	}
}
