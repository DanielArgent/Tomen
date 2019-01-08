using System;

namespace Lumen.Tomen {
	public class TomenString : ITomenValue {
		public String Value { get; set; }

		public TomenString(String value) {
			this.Value = value;
		}

		public override String ToString() {
			return "\"" + this.Value.Replace("\"", "\\\"") + "\"";
		}
	}
}
