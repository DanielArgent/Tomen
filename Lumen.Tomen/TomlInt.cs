using System;

namespace Lumen.Tomen {
	public class TomlInt : ITomlValue {
		public Int64 Value { get; }

		public TomlInt(Int64 value) {
			this.Value = value;
		}

		public override String ToString() {
			return this.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
		}
	}
}
