using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ringing_Map_Framework {
	public class RNG {
		private ulong state;

		public ulong Next () {
			ulong x = state;

			x ^= x >> 12;
			x ^= x << 25;
			x ^= x >> 27;

			state = x;

			return x * 0x2545F4914F6CDD1D;
		}

		public uint Next (uint max) => (uint)Next () % max;

		public int Next (int max) => (int)Next ((uint)max);

		public ulong Next (ulong max) => Next () % max;

		public RNG (int seed) {
			Random random = new Random (seed); // HAHA lol.

			state = ((ulong)random.Next () << 32) + (ulong)random.Next ();
		}
	}
}
