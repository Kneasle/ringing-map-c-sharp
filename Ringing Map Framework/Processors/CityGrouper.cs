using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ringing_Map_Framework.Processors {
	public static class CityGrouper {
		private static void GroupCitiesInSegment (MapSegment segment) {
			Console.WriteLine (segment.name);

			// Filter groups
			List<int> [] groups = Utils
				.GroupTowers (segment, Constants.city_group_distance_threshold)
				.Where (g => g.Count >= Constants.city_population_threshold)
				.ToArray ();

			// Debug print
			Console.WriteLine (groups.Length);
			foreach (List<int> group in groups) {
				Console.WriteLine (" >> " + string.Join (", ", group.Select (x => segment.towers [x].place)));
			}
			
			// Remove towers from the segment and add them to their own city segments
			List<int> towers_to_remove = new List<int> ();

			foreach (List<int> g in groups) {
				segment.cities.Add (new City (g.Select (x => segment.towers [x]).ToList ()));

				towers_to_remove.AddRange (g);
			}

			segment.RemoveTowers (towers_to_remove);
		}

		public static void GroupCities (Map map) {
			foreach (MapSegment segment in map.segments) {
				GroupCitiesInSegment (segment);
			}
		}
	}
}
