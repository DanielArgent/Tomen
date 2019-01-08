using System;

namespace Lumen.Tomen {
	internal class QuotedKey : ITomenKey {
		public String Value { get; set; }

		public QuotedKey(String value) {
			this.Value = value;
		}

		public override String ToString() {
			return "\"" + this.Value.Replace("\"", "\\\"") + "\"";
		}
	}
}
