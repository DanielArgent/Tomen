﻿using System;

namespace Lumen.Tomen {
	/// <summary> Tomen string </summary>
	public class TomenString : ITomenValue {
		public String Value { get; set; }

		/// <summary> Creates a Tomen string from System.String </summary>
		/// <param name="value"> Value to wrap </param>
		public TomenString(String value) {
			this.Value = value;
		}

		/// <summary> Convert to writable form </summary>
		/// <returns></returns>
		public override String ToString() {
			return "\"" + this.Value.Replace("\"", "\\\"") + "\"";
		}
	}
}
