using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using LinAlg2D;
using System.Drawing;
using System.Windows.Forms;

namespace Ringing_Map_Framework {
	public class Tower {
		public int num_bells;
		public Vector2 position;
		public string id;
		public string place;
		public string diocese;
		public string dedication;
		public bool ringable;

		public Chunk chunk;

		public string tint = "#ff0000";
		public string override_tint = null;

		public int index;

		public int label_preference = 0;

		public bool name_by_dedication = false;
		public string name => name_by_dedication ? dedication : place;

		public Rect2 affected_region => Rect2.FromCentreAndSize (map_position, Vector2.one * Constants.point_radius_outer * 2 + label_size * 2);

		public Vector2 map_position;
		public Vector2 label_size;

		public Rect2 GetLabelRectByPreference (int preference) => 
			Rect2.FromCentreAndSize (
				map_position + Constants.tower_preference_positions [preference] * 
					(Vector2.one * Constants.point_radius_outer + label_size / 2), 
				label_size
			);

		public void ComputeLabelSize () {
			Size size = TextRenderer.MeasureText (name, new Font (Constants.label_font_family, Constants.label_font_size * 100));

			label_size = new Vector2 (size.Width, size.Height) / 100f * 0.7f;
		}

		public override int GetHashCode () => id.GetHashCode ();

		public override bool Equals (object obj) {
			if (obj == null)
				return false;

			if (obj.GetType () != typeof (Tower))
				return false;

			Tower t = obj as Tower;

			return t.id == id;
		}

		public IEnumerable<XElement> GenerateXML (bool is_background) {
			int points = num_bells <= 6 ? num_bells : num_bells / 2;

			string color = ringable ? Constants.color_tower_ringable : Constants.color_tower_unringable;

			if (MapToggles.tower_points) {
				// Outer polygon
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
					new XAttribute ("fill", is_background ? Constants.color_background_fill_point : "none"),
					new XAttribute ("stroke-linejoin", "round")
				);

				// Inner polygon
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

			// Label
			if (MapToggles.tower_labels && label_preference != -1) {
				Vector2 pref_offset = Constants.tower_preference_positions [label_preference];
				Rect2 pref_rect = GetLabelRectByPreference (label_preference);

				if (is_background) {
					yield return new XElement (
						"rect",
						new XAttribute ("x", pref_rect.min_x + Constants.label_x_correction),
						new XAttribute ("y", pref_rect.min_y - label_size.y * Constants.label_height_correction),
						new XAttribute ("width", label_size.x),
						new XAttribute ("height", label_size.y),
						new XAttribute ("fill", Constants.color_background_fill_point)
					);
				} else {
					yield return new XElement (
						"text",
						new XAttribute ("x", pref_rect.min_x),
						new XAttribute ("y", pref_rect.max_y),
						new XAttribute ("font-family", Constants.label_font_family),
						new XAttribute ("font-size", Constants.label_font_size),
						new XAttribute ("font-style", Constants.label_font_style),
						new XAttribute ("fill", color),
						name
					);
				}
			}
		}

		public IEnumerable<XMLNode> GenerateXMLFast (bool is_background) {
			int points = num_bells <= 6 ? num_bells : num_bells / 2;

			string color = ringable ? Constants.color_tower_ringable : Constants.color_tower_unringable;

			#region Point
			if (MapToggles.tower_points) {
				// Outer polygon
				yield return new XMLNode (
					"polygon",
					new Dictionary<string, object> {
						{
							"points",
							string.Join (
								" ",
								PolygonPoints (points, Constants.point_radius_outer)
									.Select (p => p + map_position)
									.Select (p => p.x + "," + p.y)
							)
						},
						{ "stroke-width", Constants.line_width_tower_points },
						{ "stroke", is_background ? "none" : color },
						{ "fill", is_background ? Constants.color_background_fill_point : "none" },
						{ "stroke-linejoin", "round" }
					}
				);

				// Inner polygon
				if (num_bells > 6 && !is_background) {
					yield return new XMLNode (
						"polygon",
						new Dictionary<string, object> {
							{
								"points",
								string.Join (
									" ",
									PolygonPoints (points, Constants.point_radius_inner)
										.Select (p => p + map_position)
										.Select (p => p.x + "," + p.y)
								)
							},
							{ "stroke-width", Constants.line_width_tower_points },
							{ "stroke", color },
							{ "fill", color },
							{ "stroke-linejoin", "round" }
						}
					);
				}
			}
			#endregion

			#region Label
			if (MapToggles.tower_labels && label_preference != -1) {
				Rect2 pref_rect = GetLabelRectByPreference (label_preference);
				
				if (is_background) {
					yield return new XMLNode (
						"rect",
						new Dictionary<string, object> {
							{ "x", pref_rect.min_x },
							{ "y", pref_rect.min_y },
							{ "width", label_size.x },
							{ "height", label_size.y },
							{ "fill", MapToggles.tower_label_tint ? (override_tint ?? tint) + "77" : Constants.color_background_fill_label }
						}
					);
				} else {
					if (MapToggles.tower_label_text) {
						yield return new XMLNode (
							"text",
							new Dictionary<string, object> {
								{"x", pref_rect.min_x - Constants.label_x_correction },
								{ "y", pref_rect.max_y + label_size.y * Constants.label_height_correction },
								{ "font-family", Constants.label_font_family },
								{ "font-size", Constants.label_font_size },
								{ "font-style", Constants.label_font_style },
								{ "fill", color }
							},
							name
						);
					}
				}
			}
			#endregion
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

		public Tower (string data, char delimiter = '|') {
			string [] parts = data.Split (delimiter);

			num_bells = int.Parse (parts [0]);
			position = new Vector2 (float.Parse (parts [1]), float.Parse (parts [2]));
			id = parts [3];
			place = parts [4];
			diocese = parts [5];
			dedication = parts [6];
			ringable = parts [7] == "True";
		}

		public Tower (XElement e) {
			string Attr (string n) => e.Attribute (n).Value;

			num_bells = int.Parse (Attr ("bells"));
			position = new Vector2 (float.Parse (Attr ("x")), float.Parse (Attr ("y")));
			id = Attr ("ID");
			place = Attr ("place");
			diocese = Attr ("diocese");
			dedication = Attr ("dedication");
			ringable = Attr ("ringable") == "true";
		}

		public static Vector2 Centre (Tower [] towers) {
			float sum_x = 0f;
			float sum_y = 0f;

			foreach (Tower tower in towers) {
				sum_x += tower.map_position.x;
				sum_y += tower.map_position.y;
			}

			return new Vector2 (sum_x / towers.Length, sum_y / towers.Length);
		}

		public static Vector2 Centre (List<Tower> towers) {
			float sum_x = 0f;
			float sum_y = 0f;

			foreach (Tower tower in towers) {
				sum_x += tower.map_position.x;
				sum_y += tower.map_position.y;
			}

			return new Vector2 (sum_x / towers.Count, sum_y / towers.Count);
		}

		static IEnumerable<Vector2> PolygonPoints (int points, float radius = 1f) {
			for (int i = 0; i < points; i++) {
				double a = i * Math.PI * 2f / points;

				yield return new Vector2 ((float)Math.Sin (a) * radius, -(float)Math.Cos (a) * radius);
			}
		}
	}
}
