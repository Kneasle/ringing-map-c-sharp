using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LinAlg2D;

namespace Ringing_Map {
	public class MapSegment {
		public string name;

		public Tower [] towers;
		public MapShape [] coastline_shapes;

		public Rect2 map_clip_rect;
		public Rect2 map_tower_rect;
		public Rect2 bound_rect;

		public Transform transform;

		public XElement GenerateXML () {
			if (transform == null)
				return new XElement ("g");

			XElement group = new XElement (
				"g",
				new XElement (
					"rect",
					new XAttribute ("x", map_clip_rect.x),
					new XAttribute ("y", map_clip_rect.y),
					new XAttribute ("width", map_clip_rect.w),
					new XAttribute ("height", map_clip_rect.h),
					new XAttribute ("fill", "#ddd")
				),
				new XElement (
					"rect",
					new XAttribute ("x", map_tower_rect.x),
					new XAttribute ("y", map_tower_rect.y),
					new XAttribute ("width", map_tower_rect.w),
					new XAttribute ("height", map_tower_rect.h),
					new XAttribute ("fill", "#eee")
				),
				new XElement (
					"text",
					new XAttribute ("x", map_clip_rect.centre.x),
					new XAttribute ("y", map_clip_rect.centre.y),
					new XAttribute ("font-size", "20"),
					new XAttribute ("text-anchor", "middle"),
					new XAttribute ("dominant-baseline", "middle"),
					name
				)
			);

			foreach (MapShape shape in coastline_shapes) {
				group.Add (shape.GenerateXML (transform, Constants.line_width_coast));
			}

			foreach (Tower tower in towers) {
				group.Add (tower.GenerateXML (true).ToArray ());
			}

			foreach (Tower tower in towers) {
				group.Add (tower.GenerateXML (false).ToArray ());
			}

			return group;
		}

		public void UpdateBounds () {
			bound_rect = Rect2.Bounds (towers.Select (x => x.position));

			transform = Transform.FromTo (bound_rect, name == "Main" ? new Rect2 (0, 0, Constants.map_width, Constants.map_height) : map_tower_rect, true);

			foreach (Tower tower in towers) {
				tower.map_position = transform.TransformPoint (tower.position);
			}
		}

		public MapSegment (string name) {
			this.name = name;

			towers = File.ReadAllLines (Constants.base_path + name + @"\towers.txt")
				.Select (x => new Tower (x))
				.ToArray ();
			coastline_shapes = File.ReadAllLines (Constants.base_path + name + @"\coastlines.txt")
				.Select (x => new MapShape (x))
				.ToArray ();

			// Transform
			string s = File.ReadAllLines (Constants.base_path + name + @"\setup.txt")
				.Where (x => x.StartsWith ("Rect Coords: "))
				.First ()
				.Substring (13);

			if (s != "Main") {
				var coords = s.Split ("|").Select (x => float.Parse (x));

				Rect2 ToRect (IEnumerable<float> xywh) {
					float [] list = xywh.Take (4).ToArray ();

					float x = list [0];
					float y = list [1];
					float w = list [2];
					float h = list [3];

					return new Rect2 (x, Constants.map_height - y - h, w, h);
				}

				map_tower_rect = ToRect (coords.TakeLast (4));
				map_clip_rect = ToRect (coords);
			}

			UpdateBounds ();
		}

		public MapSegment (Tower [] towers, Rect2 map_clip_rect, Rect2 map_tower_rect, string name = "HI") {
			this.towers = towers;
			this.map_clip_rect = map_clip_rect;
			this.map_tower_rect = map_tower_rect;
			this.name = name;

			UpdateBounds ();
		}
	}
}
