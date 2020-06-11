using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ringing_Map_Framework.Processors {
	public static class Utils {
		public static List<int> [] GroupTowers (MapSegment segment, float threshold) {
			float threshold_squared = threshold * threshold;

			// Assign towers into groups
			int [] tower_ownership = new int [segment.towers.Count];

			for (int i = 0; i < tower_ownership.Length; i++) {
				tower_ownership [i] = -1;
			}

			foreach (Chunk chunk in segment.chunks) {
				foreach (Tower tower_1 in chunk.towers) {
					foreach (Tower tower_2 in chunk.towers_to_compare) {
						if (tower_1 == tower_2)
							continue;

						if ((tower_1.map_position - tower_2.map_position).square_length < threshold_squared) {
							int i = Math.Min (tower_1.index, tower_2.index);
							int j = Math.Max (tower_1.index, tower_2.index);

							tower_ownership [j] = i;
						}
					}
				}
			}

			// Find out the index of the group containing each tower
			int number_of_groups = 0;
			int [] group_indices = new int [segment.towers.Count];

			for (int i = 0; i < segment.towers.Count; i++) {
				if (tower_ownership [i] == -1) {
					group_indices [i] = number_of_groups;

					number_of_groups++;
				} else {
					group_indices [i] = group_indices [tower_ownership [i]];
				}
			}

			// Generate the groups as lists of towers
			List<int> [] groups = new List<int> [number_of_groups];

			for (int i = 0; i < groups.Length; i++) {
				groups [i] = new List<int> ();
			}

			for (int i = 0; i < segment.towers.Count; i++) {
				groups [group_indices [i]].Add (i);
			}

			return groups;
		}
	}
}
