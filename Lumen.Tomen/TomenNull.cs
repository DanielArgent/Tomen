using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumen.Tomen {
	public class TomenNull : ITomenValue {
		public static TomenNull NULL { get; } = new TomenNull();

		private TomenNull() {
		}
	}
}
