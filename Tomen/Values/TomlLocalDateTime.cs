using System;
using System.Text.RegularExpressions;

namespace Tomen {
	public class TomlLocalDateTime : TomlValue {
		public DateTime Value { get; }

		public TomlLocalDateTime(DateTime value) {
			this.Value = value;
		}

		public override String ToString() {
			return this.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "." + this.Value.Millisecond;
		}
	}
}
