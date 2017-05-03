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
			Console.WriteLine("On which port should the server run?");
			int Port;

			while (!int.TryParse(Console.ReadLine(), out Port)) { }

			Console.WriteLine("\nStarting the server. Press 'q' tot quit the program.");

			Controller.Start(Port, new Logger());

			while (Console.ReadKey().KeyChar != 'q')
			{

			}

			Controller.Stop();
		}
	}
}