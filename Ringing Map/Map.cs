using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Ringing_Map {
	public class Map {
		public MapSegment [] segments;

		public void WriteSVG (string path) {
			string header = "<?xml version = \"1.0\" encoding = \"utf-8\" standalone = \"no\"?>\n";

			XNamespace xNamespace = "http://www.w3.org/2000/svg";
			XNamespace sodipodi = "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd";
			XNamespace inkscape = "http://www.inkscape.org/namespaces/inkscape";

			XElement root = new XElement (
				xNamespace + "svg",
				new XAttribute ("width", Constants.map_width + "mm"),
				new XAttribute ("height", Constants.map_height + "mm"),
				new XAttribute ("version", "1.1"),
				new XAttribute ("baseProfile", "full"),
				new XAttribute ("viewBox", "0 0 " + Constants.map_width + " " + Constants.map_height),
				new XAttribute ("xmlns", "http://www.w3.org/2000/svg"),
				new XAttribute (XNamespace.Xmlns + "ev", "http://www.w3.org/2001/xml-events"),
				new XAttribute (XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink"),
				new XAttribute (XNamespace.Xmlns + "sodipodi", "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"),
				new XAttribute (XNamespace.Xmlns + "inkscape", "http://www.inkscape.org/namespaces/inkscape"),
				new XElement (
					sodipodi + "namedview",
					new XAttribute (inkscape + "document-units", "mm")
				)
			);

			foreach (MapSegment segment in segments) {
				root.Add (segment.GenerateXML ());
			}

			File.WriteAllText (path, header + root.ToString ().Replace (" xmlns=\"\"", ""));
		}

		public Map () {
			segments = Directory.GetDirectories (Constants.base_path)
				.Select (x => x.Substring (x.LastIndexOf ('\\') + 1))
				.Select (x => new MapSegment (x))
				.ToArray ();
		}
	}
}
