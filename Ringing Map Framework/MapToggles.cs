using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ringing_Map_Framework {
	public static class MapToggles {
		public static bool rect_outlines = false;
		public static bool chunk_outlines = false;
		public static bool coastlines = true;
		public static bool region_names = false;

		public static bool city_blobs = true;

		public static bool clipping = true;

		public static bool tower_master = true;

		public static bool tower_labels = true;
		public static bool tower_label_text = true;
		public static bool tower_label_tint = true;
		public static bool tower_points = true;
		public static bool tower_backgrounds = true;

		public static string bit_mask_string =>
			  (coastlines ? "c" : "")
			+ (region_names ? "n" : "")
			+ (clipping ? "p" : "")
			+ (rect_outlines ? "r" : "")
			+ (city_blobs ? "y" : "")
			+ (chunk_outlines ? "C" : "")
			+ (tower_master ?
				"["
				+ (tower_backgrounds ? "b" : "")
				+ (tower_labels ? "l" : "")
				+ (tower_points ? "p" : "")
				+ (tower_label_text ? "t" : "")
				+ (tower_label_tint ? "T" : "")
				+ "]"
			: "");
	}
}
