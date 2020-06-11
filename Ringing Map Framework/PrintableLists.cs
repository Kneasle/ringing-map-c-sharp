using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ringing_Map_Framework {
	public static class PrintableLists {
		public static string FullString<T> (this IEnumerable<T> list) => "[" + string.Join (", ", list.Select (x => x.ToString ())) + "]";
	}
}
