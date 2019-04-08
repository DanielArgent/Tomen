using System;

namespace Lumen.Tomen { 
	/// <summary> Toml logical value (true or false) </summary>
	public class TomlBool : ITomlValue {
		/// <summary> Internal value </summary>
		public Boolean Value { get; }

		public TomlBool(Boolean value) {
			this.Value = value;
		}

		public override String ToString() {
			return this.Value.ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower();
		}
	}
}
