using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LinAlg2D {
	public struct Rect2Int {
		public Vector2Int position;
		public Vector2Int size;

		public int x => position.x;
		public int y => position.y;
		public int w => size.x;
		public int h => size.y;

		public Vector2Int top_left => new Vector2Int (min_x, min_y);
		public Vector2Int top_right => new Vector2Int (max_x, min_y);
		public Vector2Int bottom_left => new Vector2Int (min_x, max_y);
		public Vector2Int bottom_right => new Vector2Int (max_x, max_y);

		public Vector2Int tl => top_left;
		public Vector2Int tr => top_right;
		public Vector2Int bl => bottom_left;
		public Vector2Int br => bottom_right;
		
		public Vector2Int min => position;
		public Vector2Int max => position + size;

		public int min_x => position.x;
		public int min_y => position.y;

		public int max_x => position.x + size.x;
		public int max_y => position.y + size.y;

		public Vector2 centre => position + size * .5f;

		public override string ToString () => "(" + position.x + ", " + position.y + ", " + size.x + ", " + size.y + ")";

		public bool IsPointInside (Vector2Int p) => p.x <= max_x && p.x >= min_x && p.y <= max_y && p.y >= max_y;
		public bool IsPointInside (Vector2 p) => p.x <= max_x && p.x >= min_x && p.y <= max_y && p.y >= max_y;

		public Rect2Int (Vector2Int position, Vector2Int size) {
			this.position = position;
			this.size = size;
		}

		public Rect2Int (int x, int y, int w, int h) {
			position = new Vector2Int (x, y);
			size = new Vector2Int (w, h);
		}

		public static Rect2Int unit => new Rect2Int (0, 0, 1, 1);

		public static Rect2Int Bounds (IEnumerable <Vector2Int> points) {
			bool is_empty = true;

			int min_x = int.MaxValue;
			int min_y = int.MaxValue;
			int max_x = int.MinValue;
			int max_y = int.MinValue;

			foreach (Vector2Int point in points) {
				is_empty = false;

				min_x = Math.Min (min_x, point.x);
				max_x = Math.Max (max_x, point.x);
				min_y = Math.Min (min_y, point.y);
				max_y = Math.Max (max_y, point.y);
			}

			if (is_empty)
				throw new Exception ("Can't find the bounds of an empty list of points.");

			return FromBounds (min_x, min_y, max_x, max_y);
		}

		public static Rect2Int FromBounds (Vector2Int min, Vector2Int max) => new Rect2Int (min, max - min);
		public static Rect2Int FromBounds (int min_x, int min_y, int max_x, int max_y) => FromBounds (new Vector2Int (min_x, min_y), new Vector2Int (max_x, max_y));
	}
}
