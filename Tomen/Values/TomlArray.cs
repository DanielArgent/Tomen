using System;
using System.Collections.Generic;

namespace Tomen {
	/// <summary> Toml array object </summary>
	public class TomlArray : TomlValue {
		/// <summary> Values of array </summary>
		public List<TomlValue> Value { get; }

		public TomlArray(List<TomlValue> value) {
			this.Value = value;
		}

		public override String ToString() {
			return $"[{String.Join(", ", this.Value)}]";
		}
	}
}
