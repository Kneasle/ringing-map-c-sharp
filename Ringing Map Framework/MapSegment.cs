using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LinAlg2D;
using Newtonsoft.Json;

namespace Ringing_Map_Framework {
	public class MapSegment {
		public string name;
		public string escaped_name => name.Replace ("&", "&amp;");

		public List<Tower> towers;
		public MapShape [] coastline_shapes;

		public Rect2 map_clip_rect;
		public Rect2 map_tower_rect;
		public Rect2 bound_rect;

		public Transform transform;

		public Vector2Int [] filled_chunks;
		public Chunk [,] chunks;

		public List<City> cities = new List<City> ();

		public void RemoveTowers (IEnumerable<int> indices) {
			List<int> towers_to_remove = indices.ToList ();

			towers_to_remove.Sort ();
			towers_to_remove.Reverse ();

			foreach (int i in towers_to_remove) {
				towers [i].chunk.towers.Remove (towers [i]);
				towers.RemoveAt (i);
			}

			// Update tower indices
			for (int i = 0; i < towers.Count; i++) {
				towers [i].index = i;
			}
		}

		public void GenerateChunks (bool print_debug = false) {
			Rect2Int chunk_rect = (map_tower_rect / Chunk.chunk_size).ExpandToInt ();

			Vector2 top_left_corner = chunk_rect.min * Chunk.chunk_size;

			chunks = new Chunk [chunk_rect.w, chunk_rect.h];
			HashSet<Vector2Int> filled_chunks_temp = new HashSet<Vector2Int> ();

			for (int i = 0; i < chunk_rect.w; i++) {
				for (int j = 0; j < chunk_rect.h; j++) {
					chunks [i, j] = new Chunk (new Vector2Int (i, j));
				}
			}

			// Put every tower into the correct chunk
			foreach (Tower tower in towers) {
				Vector2Int chunk_coord = ((tower.map_position - top_left_corner) / Chunk.chunk_size).floor;

				if (chunk_coord.x >= chunk_rect.w)
					chunk_coord.x--;
				if (chunk_coord.y >= chunk_rect.h)
					chunk_coord.y--;

				filled_chunks_temp.Add (chunk_coord);

				Chunk chunk = chunks [chunk_coord.x, chunk_coord.y];

				tower.chunk = chunk;
				chunk.towers.Add (tower);
			}

			// Populate chunk neighbours
			filled_chunks = filled_chunks_temp.ToArray ();

			Vector2Int [] directions = new Vector2Int [] {
				new Vector2Int (-1, -1),
				new Vector2Int (0, -1),
				new Vector2Int (1, -1),
				new Vector2Int (-1, 0),
				new Vector2Int (1, 0),
				new Vector2Int (-1, 1),
				new Vector2Int (0, 1),
				new Vector2Int (1, 1)
			};

			foreach (Vector2Int coord in filled_chunks) {
				foreach (Vector2Int offset in directions) {
					Vector2Int neighbour_coord = coord + offset;

					if (filled_chunks_temp.Contains (neighbour_coord)) {
						chunks [coord.x, coord.y].populated_neighbours.Add (chunks [neighbour_coord.x, neighbour_coord.y]);
					}
				}
			}

			// Print out the chunk info
			if (print_debug) {
				Console.WriteLine (name + ": " + chunk_rect);

				for (int j = 0; j < chunk_rect.h; j++) {
					for (int i = 0; i < chunk_rect.w; i++) {
						int c = chunks [i, j].populated_neighbours.Count;

						string s = c.ToString ();

						if (chunks [i, j].towers.Count == 0)
							s = "";

						Console.Write (s + new string (' ', 3 - s.Length));
					}

					Console.Write ("\n");
				}
			}
		}

		public XElement GenerateXML () {
			if (transform == null)
				return new XElement ("g");

			XElement group = new XElement (
				"g"
			);

			if (MapToggles.rect_outlines) {
				if (name != "Main") {
					group.Add (new XElement (
						"rect",
						new XAttribute ("x", map_clip_rect.x),
						new XAttribute ("y", map_clip_rect.y),
						new XAttribute ("width", map_clip_rect.w),
						new XAttribute ("height", map_clip_rect.h),
						new XAttribute ("fill", "#ddd")
					));
					group.Add (new XElement (
						"rect",
						new XAttribute ("x", map_tower_rect.x),
						new XAttribute ("y", map_tower_rect.y),
						new XAttribute ("width", map_tower_rect.w),
						new XAttribute ("height", map_tower_rect.h),
						new XAttribute ("fill", "#eee")
					));
				}
			}

			if (MapToggles.region_names) {
				group.Add (new XElement (
					"text",
					new XAttribute ("x", map_clip_rect.centre.x),
					new XAttribute ("y", map_clip_rect.centre.y),
					new XAttribute ("font-size", "20"),
					new XAttribute ("text-anchor", "middle"),
					new XAttribute ("dominant-baseline", "middle"),
					name
				));
			}

			if (MapToggles.coastlines) {
				foreach (MapShape shape in coastline_shapes) {
					group.Add (shape.GenerateXML (transform, Constants.line_width_coast));
				}
			}

			if (MapToggles.tower_master) {
				if (MapToggles.tower_backgrounds) {
					foreach (Tower tower in towers) {
						group.Add (tower.GenerateXML (true));
					}
				}

				foreach (Tower tower in towers) {
					group.Add (tower.GenerateXML (false));
				}
			}

			return group;
		}

		public XMLNode GenerateXMLFast () {
			if (transform == null)
				return new XMLNode ("g");

			XMLNode group = new XMLNode (
				"g"
			);

			string clip_path_name = escaped_name;

			if (MapToggles.clipping) {
				group.AddChild (new XMLNode (
					"clipPath",
					new Dictionary<string, object> {
						{ "id", clip_path_name }
					},
					new XMLNode [] {
						new XMLNode (
							"rect",
							new Dictionary<string, object> {
								{ "x", map_clip_rect.x },
								{ "y", map_clip_rect.y },
								{ "width", map_clip_rect.w },
								{ "height", map_clip_rect.h }
							}
						)
					}
				));
			}

			if (MapToggles.rect_outlines) {
				if (name != "Main") {
					group.AddChild (new XMLNode (
						"rect",
						new Dictionary<string, object> {
							{ "x", map_clip_rect.x },
							{ "y", map_clip_rect.y },
							{ "width", map_clip_rect.w },
							{ "height", map_clip_rect.h },
							{ "fill", "#ddd" }
						}
					));
					group.AddChild (new XMLNode (
						"rect",
						new Dictionary<string, object> {
							{ "x", map_tower_rect.x },
							{ "y", map_tower_rect.y },
							{ "width", map_tower_rect.w },
							{ "height", map_tower_rect.h },
							{ "fill", "#eee" }
						}
					));
				}
			}
			
			if (MapToggles.region_names) {
				group.AddChild (new XMLNode (
					"text",
					new Dictionary<string, object> {
						{ "x", map_clip_rect.centre.x },
						{ "y", map_clip_rect.centre.y },
						{ "font-size", "20" },
						{ "text-anchor", "middle" },
						{ "dominant-baseline", "middle" }
					},
					escaped_name
				));
			}
			
			if (MapToggles.coastlines) {
				foreach (MapShape shape in coastline_shapes) {
					group.AddChild (shape.GenerateXMLFast (transform, Constants.line_width_coast, MapToggles.clipping ? clip_path_name : null));
				}
			}

			if (MapToggles.city_blobs) {
				foreach (City city in cities) {
					Vector2 centre = Tower.Centre (city.towers);

					group.AddChild (
						new XMLNode (
							"circle",
							new Dictionary<string, object> {
								{ "cx", centre.x },
								{ "cy", centre.y },
								{ "r", 2f }
							}
						)
					);
				}
			}
			
			if (MapToggles.tower_master) {
				if (MapToggles.tower_backgrounds) {
					foreach (Tower tower in towers) {
						group.AddChildren (tower.GenerateXMLFast (true));
					}
				}

				foreach (Tower tower in towers) {
					group.AddChildren (tower.GenerateXMLFast (false));
				}
			}

			if (MapToggles.chunk_outlines) {
				foreach (Vector2Int coord in filled_chunks) {
					group.AddChild (chunks [coord.x, coord.y].GenerateXMLFast ((map_tower_rect.min / Chunk.chunk_size).floor));
				}
			}

			return group;
		}

		public void UpdateBounds () {
			bound_rect = Rect2.Bounds (towers.Select (x => x.position));

			transform = Transform.FromTo (bound_rect, map_tower_rect, true);

			foreach (Tower tower in towers) {
				tower.map_position = transform.TransformPoint (tower.position);
			}
		}

		public MapSegment (string name) {
			this.name = name;

			towers = XElement.Load (Constants.input_path + name + @"\towers.xml").Descendants ()
				.Select (x => new Tower (x))
				.ToList ();

			coastline_shapes = File.ReadAllLines (Constants.input_path + name + @"\coastlines.txt")
				.Select (x => new MapShape (x))
				.ToArray ();

			// Assign indices
			for (int i = 0; i < towers.Count; i++) {
				towers [i].index = i;
			}

			// Transform
			string rect_path = Constants.input_path + name + @"\rects.xml";

			if (File.Exists (rect_path)) {
				XElement root = XElement.Load (rect_path);

				Rect2 ToRect (XElement element) {
					float Attr (string n) => float.Parse (element.Attribute (n).Value);

					float x = Attr ("x");
					float y = Attr ("y");
					float w = Attr ("width");
					float h = Attr ("height");

					// We need invert the y-coordinate of the rectangle because svg has the y-axis numbers
					// the 'wrong' way up.
					return new Rect2 (x, Constants.map_height - y - h, w, h);
				}

				foreach (XElement element in root.Elements ()) {
					if (element.Name == "towerRect")
						map_tower_rect = ToRect (element);
					if (element.Name == "clippingRect")
						map_clip_rect = ToRect (element);
				}
			} else {
				map_tower_rect = new Rect2 (0, 0, Constants.map_width, Constants.map_height).Shrink (5f);
				map_clip_rect = new Rect2 (0, 0, Constants.map_width, Constants.map_height);
			}

			UpdateBounds ();
		}

		public MapSegment (List<Tower> towers, Rect2 map_clip_rect, Rect2 map_tower_rect, string name = "HI") {
			this.towers = towers;
			this.map_clip_rect = map_clip_rect;
			this.map_tower_rect = map_tower_rect;
			this.name = name;

			UpdateBounds ();
		}
	}
}

