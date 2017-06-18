using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindowsConsoleServer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("\nStarting the server. Press 'q' tot quit the program.");

			Controller.Start(5000, new Logger());

			while (Console.ReadKey().KeyChar != 'q')
			{

			}

			Controller.Stop();
		}
	}
}