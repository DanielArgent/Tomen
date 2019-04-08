using System;

namespace Lumen.Tomen {
	/// <summary> Toml string </summary>
	public class TomlString : ITomlValue {
		public String Value { get; set; }

		/// <summary> Creates a Toml string from System.String </summary>
		public TomlString(String value) {
			this.Value = value;
		}

		/// <summary> Convert to valid literal </summary>
		public override String ToString() {
			return Lexer.Normalize(this.Value);
		}
	}
}
