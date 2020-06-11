using System;
using System.IO;
using System.Linq;

namespace Ringing_Map {
	class Program {
		static void Main (string [] args) {
			Console.WriteLine ("Reading files...");

			Map map = new Map ();

			Console.WriteLine ("Writing SVG");

			map.WriteSVG (Constants.base_path + "output.svg");

			Console.WriteLine ("Done");

			Console.WriteLine ("\nPress any key to exit.");
			Console.ReadKey ();
		}
	}
}
