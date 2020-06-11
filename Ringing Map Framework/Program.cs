using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Ringing_Map_Framework.Processors;
using System.IO;

namespace Ringing_Map_Framework {
	class Program {
		static Stopwatch stopwatch;
		static double total_time = 0;

		static void LogTime () {
			double t = stopwatch.Elapsed.TotalSeconds;

			Console.WriteLine ("\n(time = " + t + "s)\n\n\n\n");

			total_time += t;

			stopwatch.Restart ();
		}

		static void Main (string [] args) {
			stopwatch = Stopwatch.StartNew ();

			string path = Constants.output_path + "output-" + MapToggles.bit_mask_string + ".svg";

			Console.WriteLine ("Reading files...");
			Map map = new Map ();
			LogTime ();

			Console.WriteLine ("Chunking...");
			Chunker.GenerateChunks (map);
			LogTime ();

			Console.WriteLine ("Grouping Cities...");
			CityGrouper.GroupCities (map);
			LogTime ();

			Console.WriteLine ("Naming Towers...");
			Namer.NameTowers (map);
			LogTime ();

			Console.WriteLine ("Nudging towers...");
			Nudger.NudgeTowers (map);
			LogTime ();

			Console.WriteLine ("Placing Labels...");
			LabelPlacer.PlaceLabels (map);
			LogTime ();

			Console.WriteLine ("Generating SVG...");
			string contents = SVGWriter.GenerateSVG (map, false);
			LogTime ();

			Console.WriteLine ("Writing File...");
			File.WriteAllText (path, contents);
			LogTime ();

			Console.WriteLine ("Done");

			Console.WriteLine (" >> Saved to: " + path);

			Console.WriteLine (total_time + "s total used.");

			Console.WriteLine ("\nPress any key to exit.");
			Console.ReadKey ();
		}
	}
}
