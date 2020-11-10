using System;
using System.Text.RegularExpressions;

namespace Tomen {
	public class TomlDateTimeOffset : TomlValue {
		public DateTimeOffset Value { get; }

		public TomlDateTimeOffset(DateTimeOffset value) {
			this.Value = value;
		}

		public override String ToString() {
			return this.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
		}
	}
}
