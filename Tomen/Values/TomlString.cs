using System;

namespace Tomen {
	/// <summary> Toml string </summary>
	public class TomlString : TomlValue {
		public String Value { get; set; }

		/// <summary> Creates a Toml string from System.String </summary>
		public TomlString(String value) {
			this.Value = value;
		}

		/// <summary> Convert to valid literal </summary>
		public override String ToString() {
			return $@"""{this.Value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\f", "\\f").Replace("\t", "\\t").Replace("\n", "\\n").Replace("\b", "\\b").Replace(Environment.NewLine, "\\r\\n")}""";
		}
	}
}
