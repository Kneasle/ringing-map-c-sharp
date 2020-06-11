using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ringing_Map_Framework.Processors {
	public class Namer {
		public void ComputeNames (MapSegment segment) {
			if (Constants.naming_distance_threshold > Chunk.chunk_size.y)
				throw new Exception ();

			float threshold_squared = Constants.naming_distance_threshold * Constants.naming_distance_threshold;

			foreach (Chunk chunk in segment.chunks) {
				foreach (Tower tower_2 in chunk.towers_to_compare) {
					foreach (Tower tower_1 in chunk.towers) {
						if (tower_1 == tower_2)
							continue;

						if (tower_1.place == tower_2.place) {
							if ((tower_1.map_position - tower_2.map_position).square_length <= threshold_squared) {
								tower_1.name_by_dedication = true;
								tower_2.name_by_dedication = true;
							}
						}
					}
				}
			}
		}

		public static void NameTowers (Map map) {
			foreach (MapSegment segment in map.segments) {
				new Namer ().ComputeNames (segment);
			}
		}
	}
}
