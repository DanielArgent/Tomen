using System;

namespace Lumen.Tomen {
	public class TomlDouble : ITomlValue {
		public Double Value { get; }

		public TomlDouble(Double value) {
			this.Value = value;
		}

		public override String ToString() {
			if (Double.IsNegativeInfinity(this.Value)) {
				return "-inf";
			}

			if (Double.IsPositiveInfinity(this.Value)) {
				return "+inf";
			}

			if (Double.IsNaN(this.Value)) {
				return "nan";
			}

			return this.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
		}
	}
}
