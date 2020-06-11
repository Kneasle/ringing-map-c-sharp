using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinAlg2D;

namespace Ringing_Map_Framework.Processors {
	public static class LabelPlacer {
		private static void PlaceLabels (MapSegment segment) {
			Console.WriteLine ("==== " + segment.name + " ====");

			int num_prefs = Constants.tower_preference_positions.Length;

			// My Linear Algebra library is apparently quite slow, so if you cache these, then it makes a 
			// massive increase to the program's speed, particularly in the Simulated Annealing stage.
			Rect2 [] influenced_region = new Rect2 [segment.towers.Count];
			Rect2 [] point_rects = new Rect2 [segment.towers.Count];
			Rect2 [,] label_pref_rects = new Rect2 [segment.towers.Count, num_prefs];
			Vector2 [,] label_pref_rect_centres = new Vector2 [segment.towers.Count, num_prefs];

			#region Populate caches
			for (int i = 0; i < segment.towers.Count; i++) {
				segment.towers [i].ComputeLabelSize ();

				for (int j = 0; j < num_prefs; j++) {
					label_pref_rects [i, j] = segment.towers [i].GetLabelRectByPreference (j);
					label_pref_rect_centres [i, j] = label_pref_rects [i, j].centre;
				}

				influenced_region [i] = segment.towers [i].affected_region;
				point_rects [i] = Rect2.FromCentreAndSize (segment.towers [i].map_position, Vector2.one * Constants.point_radius_outer * 2);

				if (influenced_region [i].w >= Chunk.chunk_size.x)
					Console.WriteLine ("!!! CHUNK SIZE NOT WIDE ENOUGH !!! : " + segment.towers [i].name);

				if (influenced_region [i].h >= Chunk.chunk_size.y)
					Console.WriteLine ("!!! CHUNK SIZE NOT TALL ENOUGH !!! : " + segment.towers [i].name);
			}
			#endregion

			/* Note that this list/relation is symmetric; if (a, b) is in the list then so is (b, a).
			 * This way, in order to check every combination symmetrically, only one iteration over the list
			 * is required with no code duplication.
			 */
			List<(Tower, Tower)> close_tower_pairs = new List<(Tower, Tower)> ();

			#region Generate a list of close tower pairs to speed up comparisons
			int p = 0;
			int q = 0;
			int r = 0;
			int [] ref_counts = new int [segment.towers.Count];

			foreach (Chunk chunk in segment.chunks) {
				foreach (Tower tower_2 in chunk.towers_to_compare) {
					foreach (Tower tower_1 in chunk.towers) {
						if (tower_1 == tower_2)
							continue;

						p++;

						if (Rect2.Overlaps (
							influenced_region [tower_1.index], 
							influenced_region [tower_2.index]
						)) {
							q++;

							close_tower_pairs.Add ((tower_1, tower_2));

							ref_counts [tower_1.index]++;

							if (tower_1.index < tower_2.index) {
								r++;
							}
						}
					}
				}
			}
			#endregion

			Console.WriteLine (" >> " + p + " comparisons normally.");
			Console.WriteLine (" >> " + q + " comparisons with reductions (" + r + " unique comparisons).");

			Console.WriteLine (" >> " + ref_counts.Where (x => x == 0).Count () + " towers will be completely ignored.");

			int [,] label_pref_orders = new int [segment.towers.Count, num_prefs];

			#region Decide on the order of preference for label positions
			float [,] label_pref_weights = new float [segment.towers.Count, num_prefs];

			foreach (Chunk chunk in segment.chunks) {
				foreach (Tower tower_2 in chunk.towers_to_compare) {
					foreach (Tower tower_1 in chunk.towers) {
						if (tower_1.index == tower_2.index)
							continue;

						if (Vector2.SquareDistanceBetween (tower_1.map_position, tower_2.map_position) > 900)
							continue;

						for (int i = 0; i < num_prefs; i++) {
							float d = label_pref_rects [tower_1.index, i].ClosestDistanceTo (tower_2.map_position);

							float dx = label_pref_rect_centres [tower_1.index, i].x - tower_2.map_position.x;
							float dy = label_pref_rect_centres [tower_1.index, i].y - tower_2.map_position.y;

							label_pref_weights [tower_1.index, i] += 1f / (dx * dx + dy * dy + 0.01f);
						}
					}
				}
			}

			// Sort weights
			for (int i = 0; i < segment.towers.Count; i++) {
				List<(int, float)> indices_in_order = Enumerable.Range (0, num_prefs).Select (x => (x, label_pref_weights [i, x])).ToList ();

				indices_in_order.Sort ((p1, p2) => p1.Item2.CompareTo (p2.Item2));

				for (int j = 0; j < num_prefs; j++) {
					label_pref_orders [i, j] = indices_in_order [j].Item1;
				}
			}
			#endregion

			int [] pref_bitmasks = new int [segment.towers.Count]; // Has a 1-bit whenever that preference clashes with a point

			#region Process label-to-point collisions to find which prefs just shouldn't be used
			foreach ((Tower, Tower) pair in close_tower_pairs) {
				Tower tower_1 = pair.Item1;
				Tower tower_2 = pair.Item2;

				for (int i = 0; i < num_prefs; i++) {
					if (Rect2.Overlaps (
						label_pref_rects [tower_1.index, i],
						point_rects [tower_2.index]
					)) {
						pref_bitmasks [tower_1.index] |= 1 << i;
					}
				}
			}
			#endregion

			int [] best_prefs = new int [segment.towers.Count];

			#region Find current best preferences for every tower
			for (int i = 0; i < segment.towers.Count; i++) {
				bool has_broken = false;

				for (int k = 0; k < num_prefs; k++) {
					int j = label_pref_orders [i, k];

					if ((pref_bitmasks [i] & (1 << j)) == 0) {
						best_prefs [i] = j;

						has_broken = true;

						break;
					}
				}

				if (!has_broken) {
					// Tower is unlabelable
					best_prefs [i] = -1;
				}
			}
			#endregion

			Console.WriteLine (" >> " + best_prefs.Where (x => x == -1).Count () + " towers unlabelable.");

			HashSet<int> fixed_towers = new HashSet<int> ();

			#region Fix (in place) towers which can never cause clashes or are unlabelable
			int last_fixed_towers_length = -1;

			int counter = 0;

			// This algorithm is an iterative loop which chips away at the number of towers which still need
			// to be labelled.  Each iteration, more towers are fixed in place, and this stops when an
			// iteration makes no progress, indicating that this algorithm has done as much as it can, and
			// simulated annealing has got to take over to do the rest.
			while (true) {
				bool [] has_potential_clash = new bool [segment.towers.Count];

				foreach ((Tower, Tower) pair in close_tower_pairs) {
					Tower tower_1 = pair.Item1;
					Tower tower_2 = pair.Item2;

					// If either of them are unlabelable then there can't be a potential clash
					if (best_prefs [tower_1.index] == -1 || best_prefs [tower_2.index] == -1)
						continue;

					// Convenient helper function to test the tower's preference against a given rect
					void CollideWithTower2Rect (Rect2 rect) {
						if (Rect2.Overlaps (
							label_pref_rects [tower_1.index, best_prefs [tower_1.index]],
							rect
						)) {
							has_potential_clash [tower_1.index] = true;
						}
					}

					// Run tower_1's best rect against tower_2
					if (fixed_towers.Contains (tower_2.index)) {
						CollideWithTower2Rect (label_pref_rects [tower_2.index, best_prefs [tower_2.index]]);
					} else {
						for (int i = 0; i < num_prefs; i++) {
							if ((pref_bitmasks [tower_2.index] & 1 << i) == 0) {
								CollideWithTower2Rect (label_pref_rects [tower_2.index, best_prefs [tower_2.index]]);
							}
						}
					}
				}

				// Filter out the towers which can safely be fixed
				for (int i = 0; i < segment.towers.Count; i++) {
					if (!has_potential_clash [i]) {
						if (!fixed_towers.Contains (i)) {
							// Set tint for debug purposes
							char blue  = "000000000000" [counter];
							char green = "fb73000037bf" [counter];
							char red   = "000037bfb730" [counter];

							segment.towers [i].tint = "#00ff00";
						}

						fixed_towers.Add (i);
					}
				}
				
				Console.WriteLine (" >> " + fixed_towers.Count + "/" + segment.towers.Count + " towers fixed.");

				if (fixed_towers.Count == last_fixed_towers_length)
					break;

				last_fixed_towers_length = fixed_towers.Count;
				counter++;
			}
			#endregion

			int [] current_prefs = (int [])best_prefs.Clone ();

			#region Run simulated annealing on the remaining unfixed and not unlabellable towers
			/*
			// Generate precomputed lookups to run quickly
			List<(Tower, Tower)> reduced_close_tower_pairs = close_tower_pairs.Where (pair =>
				!fixed_towers.Contains (pair.Item1.index)
				&&
				!fixed_towers.Contains (pair.Item2.index)
			).ToList ();

			int [] [] possible_label_preferences = new int [segment.towers.Count] [];
			for (int i = 0; i < segment.towers.Count; i++) {
				List<int> possible_preferences = new List<int> ();

				for (int j = 0; j < num_prefs; j++) {
					if ((pref_bitmasks [i] & 1 << j) == 0) {
						possible_preferences.Add (j);
					}
				}

				possible_label_preferences [i] = possible_preferences.ToArray ();
			}

			int [] reduced_towers = segment.towers.Where (x => !fixed_towers.Contains (x.index)).Select (x => x.index).ToArray ();
			int n_reduced_towers = reduced_towers.Length;

			// Generate initial values
			int n = segment.towers.Count - fixed_towers.Count;
			int iters = 0; // (int)Math.Pow (n, 0);

			int unlabelable_bitmask = 1 << (num_prefs + 1) - 1;

			Random rng = new Random (0);

			float current_energy = GetEnergy (current_prefs);

			// Helper functions functions
			float GetEnergy (int [] prefs) => reduced_close_tower_pairs.Select (pair => {
				Tower t1 = pair.Item1;
				Tower t2 = pair.Item2;

				int i1 = t1.index;
				int i2 = t2.index;

				Rect2 r1 = label_pref_rects [i1, prefs [i1]];
				Rect2 r2 = label_pref_rects [i2, prefs [i2]];

				return 1f / (lebel_pref_rect_centres [i1, prefs [i1]] - t2.map_position).length + (Rect2.Overlaps (r1, r2) ? 1f : 0f);
			}).Sum ();

			int [] Transpose (int [] prefs, int temp) {
				int [] new_prefs = (int [])prefs.Clone ();

				for (int i = 0; i < temp; i++) {
					int ind = rng.Next (n_reduced_towers);

					int [] possible_preferences = possible_label_preferences [reduced_towers [ind]];
					
					new_prefs [ind] = possible_preferences [rng.Next (possible_preferences.Length)];
				}

				return new_prefs;
			}
			
			Console.WriteLine (" >> " + reduced_close_tower_pairs.Count + " comparisons after fixing towers.");
			Console.WriteLine (" >> Base (naive) energy: " + current_energy);
			Console.WriteLine (" >> Running " + iters + " iterations of simulated annealing...");
			
			for (int i = 0; i < iters; i++) {
				int temperature = (int)(n * 0.1f * (1f - (float)i / iters));

				int [] new_prefs = Transpose (current_prefs, temperature);

				float energy = GetEnergy (new_prefs);

				// Always take the best one
				if (energy < current_energy) {
					current_energy = energy;
					current_prefs = new_prefs;
				}

				if ((i % 500) == 0)
					Console.WriteLine (current_energy);

				if ((i + 1) % 10000 == 0)
					Console.Write (".");
				if ((i + 1) % 50000 == 0)
					Console.Write (" ");
				if ((i + 1) % 100000 == 0)
					Console.Write ((i + 1) + "\n");
			}*/
			#endregion

			// Set preferences
			for (int i = 0; i < segment.towers.Count; i++) {
				segment.towers [i].label_preference = current_prefs [i];
			}

			Console.WriteLine ("\n\n\n\n");
		}

		public static void PlaceLabels (Map map) {
			foreach (MapSegment segment in map.segments) {
				PlaceLabels (segment);
			}
		}
	}
}
