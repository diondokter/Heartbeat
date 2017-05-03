using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsConsoleServer
{
	internal class Logger : ILogger
	{
		public void OnLogReceived(string Log)
		{
			Console.WriteLine(Log);
		}
	}
}
