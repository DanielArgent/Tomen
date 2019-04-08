using System;
using System.Collections.Generic;

namespace Lumen.Tomen {
	/// <summary> Toml array object </summary>
	public class TomlArray : ITomlValue {
		/// <summary> Values of array </summary>
		public List<ITomlValue> Value { get; }

		public TomlArray(List<ITomlValue> value) {
			this.Value = value;
		}

		public override String ToString() {
			return $"[{String.Join(", ", this.Value)}]";
		}
	}
}
