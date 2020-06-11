using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ringing_Map_Framework.Processors {
	public static class Chunker {
		public static void GenerateChunks (Map map) {
			foreach (MapSegment segment in map.segments) {
				segment.GenerateChunks ();
			}
		}
	}
}
