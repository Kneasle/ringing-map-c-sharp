using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using LinAlg2D;

namespace Ringing_Map {
	public class MapShape {
		public Vector2 [] points;
		public bool closed;

		public XElement GenerateXML (Transform t, float stroke_width) => new XElement (
			"polygon", 
			new XAttribute (
				"points",
				string.Join (" ", points.Select (t.TransformPoint).Select (p => p.x + "," + p.y))
			),
			new XAttribute ("stroke", "black"),
			new XAttribute ("fill", "none"),
			new XAttribute ("stroke-width", stroke_width),
			new XAttribute ("stroke-linejoin", "round")
		);

		public MapShape (Vector2 [] points, bool closed) {
			this.points = points;
			this.closed = closed;
		}

		public MapShape (string data) {
			string [] parts = data.Split ("|");

			closed = parts [0] == "True";

			float [] coords = parts.Skip (1).Select (x => float.Parse (x)).ToArray ();

			points = new Vector2 [coords.Length / 2];

			for (int i = 0; i < coords.Length / 2; i++) {
				points [i] = new Vector2 (coords [2 * i], coords [2 * i + 1]);
			}
		}
	}
}
