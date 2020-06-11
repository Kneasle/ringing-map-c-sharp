using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinAlg2D;

namespace Ringing_Map_Framework {
	public class Chunk {
		public Vector2Int position;
		public Rect2 rect;

		public List<Chunk> populated_neighbours = new List<Chunk> ();
		public List<Tower> towers = new List<Tower> ();

		public XMLNode GenerateXMLFast (Vector2 segment_rect_min) => new XMLNode (
			"rect",
			new Dictionary<string, object> {
				{ "fill", "none" },
				{ "stroke", "#000" },
				{ "stroke-width", 0.05f },
				{ "x", (position.x + segment_rect_min.x) * chunk_size.x },
				{ "y", (position.y + segment_rect_min.y) * chunk_size.y },
				{ "width", rect.w * chunk_size.x },
				{ "height", rect.h * chunk_size.y }
			}
		);

		public IEnumerable <Tower> towers_to_compare {
			get {
				foreach (Tower tower in towers) {
					yield return tower;
				}

				foreach (Chunk chunk in populated_neighbours) {
					foreach (Tower tower in chunk.towers) {
						yield return tower;
					}
				}
			}
		}

		public Chunk (Vector2Int position) {
			this.position = position;

			rect = new Rect2 (position, Vector2.one);
		}

		public static Vector2 chunk_size = new Vector2 (90f, 30f);
	}
}
