using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinAlg2D;

namespace Ringing_Map_Framework {
	public class City {
		public List<Tower> towers;

		public City (List<Tower> towers) {
			this.towers = towers;
		}
	}
}
