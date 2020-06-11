using System;
using System.Collections.Generic;
using System.Text;
using LinAlg2D;

namespace Ringing_Map_Framework {
	public class Transform {
		public enum TransformType {
			Squish,
			Stretch
		}

		public Vector2 pre_translation;
		public Vector2 scale = Vector2.one;
		public Vector2 post_translation;
		public bool flip_vertically = true;
		public bool flip_horizontally = false;

		public Transform inverse => new Transform (-post_translation, Vector2.one / scale, -pre_translation);

		public Vector2 TransformPoint (Vector2 v) => (v + pre_translation) * scale + post_translation;
		public Vector2 InverseTransformPoint (Vector2 v) => (v - post_translation) / scale + pre_translation;

		public Transform (Vector2 pre_translation, Vector2 scale, Vector2 post_translation) {
			this.pre_translation = pre_translation;
			this.scale = scale;
			this.post_translation = post_translation;
		}

		public static Transform FromTo (Rect2 input, Rect2 output, bool flip_vertically = false, TransformType type = TransformType.Squish) {
			float x_scale = output.size.x / input.size.x;
			float y_scale = output.size.y / input.size.y;

			float s = type == TransformType.Squish ? Math.Min (x_scale, y_scale) : Math.Max (x_scale, y_scale);

			Vector2 transformed_size = input.size * s;

			Vector2 offset = (output.size - transformed_size) / 2;

			if (flip_vertically)
				return new Transform (-input.bottom_left, new Vector2 (s, -s), output.position + offset);
			else
				return new Transform (-input.position, new Vector2 (s, s), output.position + offset);
		}
	}
}
