using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Text;
using System.Linq;
using System.Xml.Linq;
using LinAlg2D;

namespace Ringing_Map {
	public class Tower {
		public int num_bells;
		public Vector2 position;
		public string id;
		public string place;
		public string diocese;
		public string dedication;
		public bool ringable;
		
		public Vector2 map_position;

		public IEnumerable<XElement> GenerateXML (bool is_background) {
			int points = num_bells <= 6 ? num_bells : num_bells / 2;

			string color = ringable ? Constants.color_tower_ringable : Constants.color_tower_unringable;

			yield return new XElement (
				"polygon",
				new XAttribute (
					"points", 
					string.Join (
						" ", 
						PolygonPoints (points, Constants.point_radius_outer)
							.Select (p => p + map_position)
							.Select (p => p.x + "," + p.y)
					)
				),
				new XAttribute ("stroke-width", Constants.line_width_tower_points),
				new XAttribute ("stroke", is_background ? "none" : color),
				new XAttribute ("fill", is_background ? Constants.color_background_fill : "none"),
				new XAttribute ("stroke-linejoin", "round")
			);

			if (num_bells > 6 && !is_background) {
				yield return new XElement (
					"polygon",
					new XAttribute (
						"points",
						string.Join (
							" ",
							PolygonPoints (points, Constants.point_radius_inner)
								.Select (p => p + map_position)
								.Select (p => p.x + "," + p.y)
						)
					),
					new XAttribute ("stroke-width", Constants.line_width_tower_points),
					new XAttribute ("stroke", color),
					new XAttribute ("fill", color),
					new XAttribute ("stroke-linejoin", "round")
				);
			}
		}

		public Tower (int num_bells, Vector2 position, string id, string place, string diocese, string dedication, bool ringable) {
			this.num_bells = num_bells;
			this.position = position;
			this.id = id;
			this.place = place;
			this.diocese = diocese;
			this.dedication = dedication;
			this.ringable = ringable;
		}

		public Tower (string data, string delimiter = "|") {
			string [] parts = data.Split (delimiter);

			num_bells = int.Parse (parts [0]);
			position = new Vector2 (float.Parse (parts [1]), float.Parse (parts [2]));
			id = parts [3];
			place = parts [4];
			diocese = parts [5];
			dedication = parts [6];
			ringable = parts [7] == "True";
		}

		static IEnumerable<Vector2> PolygonPoints (int points, float radius = 1f) {
			for (int i = 0; i < points; i++) {
				double a = i * Math.PI * 2f / points;

				yield return new Vector2 ((float)Math.Sin (a) * radius, - (float)Math.Cos (a) * radius);
			}
		}
	}
}
