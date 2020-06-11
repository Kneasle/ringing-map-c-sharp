using System;
using System.Collections.Generic;
using System.Text;

namespace LinAlg2D {
	public struct Vector2 {
		public float x;
		public float y;

		public float length => (float)Math.Sqrt (square_length);
		public float square_length => x * x + y * y;

		public void Normalise () {
			float l = length;

			x /= l;
			y /= l;
		}

		public Vector2Int floor => new Vector2Int ((int)Math.Floor (x), (int)Math.Floor (y));
		public Vector2Int ceiling => new Vector2Int ((int)Math.Ceiling (x), (int)Math.Ceiling (y));
		public Vector2Int rounded => new Vector2Int ((int)Math.Round (x), (int)Math.Round (y));

		public override string ToString () => "(" + x + ", " + y + ")";

		public Vector2 normalised => this / length;

		public static Vector2 operator * (float s, Vector2 v) => new Vector2 (s * v.x, s * v.y);
		public static Vector2 operator * (Vector2 v, float s) => s * v;

		public static Vector2 operator * (Vector2 a, Vector2 b) => new Vector2 (a.x * b.x, a.y * b.y);
		public static Vector2 operator / (Vector2 a, Vector2 b) => new Vector2 (a.x / b.x, a.y / b.y);

		public static Vector2 operator / (Vector2 v, float s) => new Vector2 (v.x / s, v.y / s);

		public static Vector2 operator + (Vector2 a, Vector2 b) => new Vector2 (a.x + b.x, a.y + b.y);
		public static Vector2 operator - (Vector2 a, Vector2 b) => new Vector2 (a.x - b.x, a.y - b.y);

		public static Vector2 operator - (Vector2 v) => new Vector2 (-v.x, -v.y);

		public static Vector2 right => new Vector2 (1, 0);
		public static Vector2 left => new Vector2 (-1, 0);

		public static Vector2 up => new Vector2 (0, -1);
		public static Vector2 down => new Vector2 (0, 1);

		public static Vector2 zero => new Vector2 (0, 0);
		public static Vector2 one => new Vector2 (1, 1);

		public static float Dot (Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

		public static float SquareDistanceBetween (Vector2 a, Vector2 b) {
			float dx = a.x - b.x;
			float dy = a.y - b.y;

			return dx * dx + dy * dy;
		}

		public static float DistanceBetween (Vector2 a, Vector2 b) => (float)Math.Sqrt (SquareDistanceBetween (a, b));

		public Vector2 (float x, float y) {
			this.x = x;
			this.y = y;
		}
	}
}
