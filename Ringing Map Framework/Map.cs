using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Ringing_Map_Framework {
	public class Map {
		public MapSegment [] segments;

		public Map () {
			segments = Directory.GetDirectories (Constants.input_path)
				.Select (x => x.Substring (x.LastIndexOf ('\\') + 1))
				.Select (x => new MapSegment (x))
				.ToArray ();
		}
	}
}
