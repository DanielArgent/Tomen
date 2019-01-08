using System;

namespace Lumen.Tomen {
	public class TomlBool : ITomlValue {
		public Boolean Value { get; }

		public TomlBool(Boolean value) {
			this.Value = value;
		}

		public override String ToString() {
			return this.Value.ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower();
		}
	}
}
