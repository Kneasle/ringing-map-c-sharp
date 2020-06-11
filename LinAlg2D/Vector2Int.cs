using System;
using System.Collections.Generic;
using System.Text;

namespace LinAlg2D {
	public struct Vector2Int {
		public int x;
		public int y;

		public float length => (float)Math.Sqrt (square_length);
		public int square_length => x * x + y * y;

		public override string ToString () => "(" + x + ", " + y + ")";

		public Vector2 normalised => this / length;

		public static implicit operator Vector2 (Vector2Int v) => new Vector2 (v.x, v.y);

		public static Vector2 operator * (float s, Vector2Int v) => new Vector2 (s * v.x, s * v.y);
		public static Vector2 operator * (Vector2Int v, float s) => s * v;

		public static Vector2Int operator * (int s, Vector2Int v) => new Vector2Int (s * v.x, s * v.y);
		public static Vector2Int operator * (Vector2Int v, int s) => s * v;

		public static Vector2Int operator * (Vector2Int a, Vector2Int b) => new Vector2Int (a.x * b.x, a.y * b.y);
		public static Vector2 operator / (Vector2Int a, Vector2Int b) => new Vector2 ((float)a.x / b.x, (float)a.y / b.y);

		public static Vector2 operator / (Vector2Int v, float s) => new Vector2 (v.x / s, v.y / s);

		public static Vector2Int operator + (Vector2Int a, Vector2Int b) => new Vector2Int (a.x + b.x, a.y + b.y);
		public static Vector2Int operator - (Vector2Int a, Vector2Int b) => new Vector2Int (a.x - b.x, a.y - b.y);

		public static Vector2Int operator - (Vector2Int v) => new Vector2Int (-v.x, -v.y);

		public static Vector2Int right => new Vector2Int (1, 0);
		public static Vector2Int left => new Vector2Int (-1, 0);

		public static Vector2Int up => new Vector2Int (0, -1);
		public static Vector2Int down => new Vector2Int (0, 1);

		public static Vector2Int zero => new Vector2Int (0, 0);
		public static Vector2Int one => new Vector2Int (1, 1);

		public static int Dot (Vector2Int a, Vector2Int b) => a.x * b.x + a.y * b.y;

		public Vector2Int (int x, int y) {
			this.x = x;
			this.y = y;
		}
	}
}
