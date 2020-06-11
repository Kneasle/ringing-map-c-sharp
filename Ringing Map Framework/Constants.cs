using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinAlg2D;

namespace Ringing_Map_Framework {
	public static class Constants {
		public const float scale = 0.0025f; // mm per m in real life

		public const float mm_to_px = 3.7795f;

		public const float map_width = 1500f; // mm
		public const float map_height = 1500f; // mm

		public const float point_radius_inner = 0.3f; // mm
		public const float point_radius_outer = 0.75f; // mm
		public const bool point_inner_filled = true;

		public const float point_text_size = 1.6f; // mm
		public const float point_text_vertical_offset = 0.35f; // mm
		public const string point_text_font_family = "Verdana";

		public const float label_font_size = 2f; // pt
		public const string label_font_family = "Times New Roman";
		public const string label_font_style = ""; // "italic";
		public const string label_font_path = "C:/Windows/Font/timesi.ttf";
		public const float label_height = label_font_size * 0.7f;
		public const float label_height_correction = -0.15f;
		public const float label_x_correction = -0.2f; // mm

		public const float line_width_city = 0.15f; // mm
		public const float line_width_coast = 0.5f; // mm
		public const float line_width_diocese_borders = 0.15f; // mm
		public const float line_width_tower_points = 0.15f; // mm

		public const string color_background_fill_point = "#ffffff";
		public const string color_background_fill_label = "#ffffff";
		public const string color_tower_ringable = "#000000";
		public const string color_tower_unringable = "#999999";
		public const string color_city_fill = "#777777";
		public const string color_coastline = "#000000";

		public const float naming_distance_threshold = 30f; // mm

		public const float city_group_distance_threshold = 3.5f; // mm
		public const int city_population_threshold = 4;

		public const float nudging_distance_threshold = 1f;
		public const float nudged_distance = 1.2f;

		public static Vector2 [] tower_preference_positions = new Vector2 [] {
			new Vector2 (1, -1),
			new Vector2 (-1, -1),
			new Vector2 (1, 1),
			new Vector2 (-1, 1),
			new Vector2 (0, 1),
			new Vector2 (0, -1.1f),
			new Vector2 (-1, 0),
			new Vector2 (1, 0)
		};

		public static string [] rejected_diocese => new string [] {
			"PrivOwnership", "(Not Anglican)", "(RC)",
			"Trust (nonCCT)", "ChConsvnTrust", "Secular Tower",
			"ExtraParochial", "Royal Peculiar", "FriendsOfFC"
		};

		public const string input_path = @"C:\Users\kneas\Bell ringing map\C#\Input\";
		public const string output_path = @"C:\Users\kneas\Bell ringing map\C#\";
	}
}
