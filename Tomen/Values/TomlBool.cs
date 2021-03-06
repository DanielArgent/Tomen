﻿using System;

namespace Tomen { 
	/// <summary> Toml logical value (true or false) </summary>
	public class TomlBool : TomlValue {
		/// <summary> Internal value </summary>
		public Boolean Value { get; }

		public TomlBool(Boolean value) {
			this.Value = value;
		}

		public override String ToString() {
			return this.Value ? "true" : "false";
		}
	}
}
