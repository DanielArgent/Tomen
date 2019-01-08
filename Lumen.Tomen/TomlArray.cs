using System;
using System.Collections.Generic;

namespace Lumen.Tomen {
	public class TomlArray : ITomlValue {
		public List<ITomlValue> Value { get; }

		public TomlArray(List<ITomlValue> value) {
			this.Value = value;
		}

		public override String ToString() {
			return $"[{String.Join(", ", this.Value)}]";
		}
	}
}
