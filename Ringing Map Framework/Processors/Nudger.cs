using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinAlg2D;

namespace Ringing_Map_Framework.Processors {
	public static class Nudger {
		private static void NudgeTowers (MapSegment segment) {
			List<int> [] groups = Utils.GroupTowers (segment, Constants.nudging_distance_threshold);

			Console.WriteLine (segment.name);
			foreach (List<int> group in groups.Where (x => x.Count >= 2)) {
				Tower [] towers = group
					.Select (x => segment.towers [x])
					.ToArray ();

				Vector2 centre = Tower.Centre (towers);

				if (towers.Length == 2) {
					if (!towers [0].map_position.Equals (towers [1].map_position)) {
						foreach (Tower tower in towers) {
							tower.map_position = centre + (tower.map_position - centre).normalised * (Constants.nudged_distance / 2);
						}
					} else {
						Vector2 direction = Vector2.one;

						direction.Normalise ();

						towers [0].map_position = centre + direction * (Constants.nudged_distance / 2);
						towers [1].map_position = centre - direction * (Constants.nudged_distance / 2);
					}
				} else {
					Console.WriteLine ("!!!!!!! MORE THAN TWO TOWERS NOT IMPLEMENTED !!!!!");
				}
			}
		}

		public static void NudgeTowers (Map map) {
			foreach (MapSegment segment in map.segments) {
				NudgeTowers (segment);
			}
		}
	}
}
