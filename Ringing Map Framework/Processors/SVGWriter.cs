using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace Ringing_Map_Framework.Processors {
	public static class SVGWriter {
		public static string GenerateSVG (Map map, bool use_linq = false) {
			string header = "<?xml version = \"1.0\" encoding = \"utf-8\" standalone = \"no\"?>\n";

			if (use_linq) {
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

				foreach (MapSegment segment in map.segments) {
					root.Add (segment.GenerateXML ());
				}

				return header + root.ToString ().Replace (" xmlns=\"\"", "");
			} else {
				/* <svg 
						width="1500mm" 
						height="1500mm" 
						version="1.1" 
						baseProfile="full" 
						viewBox="0 0 1500 1500" 
						xmlns="http://www.w3.org/2000/svg" 
						xmlns:ev="http://www.w3.org/2001/xml-events" 
						xmlns:xlink="http://www.w3.org/1999/xlink" 
						xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd" 
						xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"
					>
				*/
				XMLNode root = new XMLNode (
					"svg",
					new Dictionary<string, object> {
						{ "width", Constants.map_width + "mm" },
						{ "height", Constants.map_height + "mm" },
						{ "version", "1.1" },
						{ "baseProfile", "full" },
						{ "viewBox", "0 0 " + Constants.map_width + " " + Constants.map_height },
						{ "xmlns", "http://www.w3.org/2000/svg" },
						{ "xmlns:ev", "http://www.w3.org/2001/xml-events" },
						{ "xmlns:xlink", "http://www.w3.org/2001/xml-events" },
						{ "xmlns:sodipodi", "http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd" },
						{ "xmlns:inkscape", "http://www.inkscape.org/namespaces/inkscape" }
					},
					map.segments.Select (x => x.GenerateXMLFast ())
				);

				return header + root.ToString ();
			}
		}
	}
}
