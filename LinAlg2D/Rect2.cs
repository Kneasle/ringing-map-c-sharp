using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LinAlg2D {
	public struct Rect2 {
		public enum RoundType {
			Floor,
			Ceil,
			Nearest
		}

		public Vector2 position;
		public Vector2 size;

		public float x => position.x;
		public float y => position.y;
		public float w => size.x;
		public float h => size.y;

		public Vector2 top_left => new Vector2 (min_x, min_y);
		public Vector2 top_right => new Vector2 (max_x, min_y);
		public Vector2 bottom_left => new Vector2 (min_x, max_y);
		public Vector2 bottom_right => new Vector2 (max_x, max_y);

		public Vector2 tl => top_left;
		public Vector2 tr => top_right;
		public Vector2 bl => bottom_left;
		public Vector2 br => bottom_right;
		
		public Vector2 min => position;
		public Vector2 max => position + size;

		public float min_x => position.x;
		public float min_y => position.y;

		public float max_x => position.x + size.x;
		public float max_y => position.y + size.y;

		public Vector2 centre => position + size * .5f;

		public Rect2 Shrink (float factor) => new Rect2 (x + factor, y + factor, w - factor * 2, h - factor * 2);
		public Rect2 Shrink (float factor_x, float factor_y) => new Rect2 (x + factor_x, y + factor_y, w - factor_x * 2, h - factor_y * 2);
		public Rect2 Shrink (Vector2 factor) => Shrink (factor.x, factor.y);

		public Rect2 Expand (float factor) => new Rect2 (x - factor, y - factor, w + factor * 2, h + factor * 2);
		public Rect2 Expand (float factor_x, float factor_y) => new Rect2 (x - factor_x, y - factor_y, w + factor_x * 2, h + factor_y * 2);
		public Rect2 Expand (Vector2 factor) => Expand (factor.x, factor.y);

		public Rect2Int ConvertToInt (RoundType min_x_type, RoundType min_y_type, RoundType max_x_type, RoundType max_y_type) {
			int Convert (float v, RoundType type) {
				switch (type) {
					case RoundType.Nearest:
						return (int)Math.Round (v);
					case RoundType.Ceil:
						return (int)Math.Ceiling (v);
					case RoundType.Floor:
						return (int)Math.Floor (v);
					default:
						return 0;
				}
			}

			return Rect2Int.FromBounds (
				Convert (min_x, min_x_type),
				Convert (min_y, min_y_type),
				Convert (max_x, max_x_type),
				Convert (max_y, max_y_type)
			);
		}

		public Rect2Int ExpandToInt () => ConvertToInt (RoundType.Floor, RoundType.Floor, RoundType.Ceil, RoundType.Ceil);
		public Rect2Int ContractToInt () => ConvertToInt (RoundType.Ceil, RoundType.Ceil, RoundType.Floor, RoundType.Floor);
		public Rect2Int RoundToInt () => ConvertToInt (RoundType.Nearest, RoundType.Nearest, RoundType.Nearest, RoundType.Nearest);

		public bool IsPointInside (Vector2 p) => p.x <= max_x && p.x >= min_x && p.y <= max_y && p.y >= max_y;

		public float ClosestDistanceTo (Vector2 p) {
			float x = p.x;
			if (p.x < min.x)
				x = min.x;
			if (p.x > max.y)
				x = max.x;

			float y = p.y;
			if (p.y < min.y)
				y = min.y;
			if (p.y > max.y)
				y = max.y;

			float dx = p.x - x;
			float dy = p.y - y;

			return (float)Math.Sqrt (dx * dx + dy * dy);
		}

		public override string ToString () => "(" + position.x + ", " + position.y + ", " + size.x + ", " + size.y + ")";

		public Rect2 (Vector2 position, Vector2 size) {
			this.position = position;
			this.size = size;
		}

		public Rect2 (float x, float y, float w, float h) {
			position = new Vector2 (x, y);
			size = new Vector2 (w, h);
		}

		public static Rect2 operator * (Rect2 r, float s) => new Rect2 (r.position * s, r.size * s);
		public static Rect2 operator * (float s, Rect2 r) => r * s;

		public static Rect2 operator / (Rect2 r, float s) => new Rect2 (r.position / s, r.size / s);
		public static Rect2 operator / (Rect2 r, Vector2 v) => new Rect2 (r.position / v, r.size / v);

		public static Rect2 operator + (Rect2 r, Vector2 v) => new Rect2 (r.position + v, r.size);
		public static Rect2 operator - (Rect2 r, Vector2 v) => new Rect2 (r.position - v, r.size);

		public static Rect2 unit => new Rect2 (0, 0, 1, 1);

		public static bool Overlaps (Rect2 a, Rect2 b) {
			if (a.max_x <= b.min_x)
				return false;

			if (a.max_y <= b.min_y)
				return false;

			if (b.max_x <= a.min_x)
				return false;

			if (b.max_y <= a.min_y)
				return false;

			return true;
		}

		public static bool OverlapsStrict (Rect2 a, Rect2 b) {
			if (a.max_x < b.min_x)
				return false;

			if (a.max_y < b.min_y)
				return false;

			if (b.max_x < a.min_x)
				return false;

			if (b.max_y < a.min_y)
				return false;

			return true;
		}

		public static Rect2 Bounds (IEnumerable <Vector2> points) {
			bool is_empty = true;

			float min_x = float.PositiveInfinity;
			float min_y = float.PositiveInfinity;
			float max_x = float.NegativeInfinity;
			float max_y = float.NegativeInfinity;

			foreach (Vector2 point in points) {
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

		public static Rect2 FromCentreAndSize (Vector2 centre, Vector2 size) => new Rect2 (centre - size / 2, size);

		public static Rect2 FromBounds (Vector2 min, Vector2 max) => new Rect2 (min, max - min);
		public static Rect2 FromBounds (float min_x, float min_y, float max_x, float max_y) => FromBounds (new Vector2 (min_x, min_y), new Vector2 (max_x, max_y));
	}
}
