using System;
using System.Text;
using System.Collections.Generic;

namespace Lumen.Tomen {
	public class TomenTable : ITomenValue {
		private readonly Dictionary<String, ITomenValue> pairs;
		public String Name { get; }

		public ITomenValue this[String name] {
			get => this.pairs[name];
			set => this.pairs[name] = value;
		}

		public TomenTable(String name) {
			this.Name = name;
			this.pairs = new Dictionary<String, ITomenValue>();
		}

		public override String ToString() {
			StringBuilder result = new StringBuilder();

			if(this.Name != null) {
				result.Append($"[{this.Name}]{Environment.NewLine}");
			}

			foreach(KeyValuePair<String, ITomenValue> i in this.pairs) {
				if (i.Value is TomenTable) {
					result.Append($"{Environment.NewLine}{i.Value}{Environment.NewLine}");
				}
				else {
					result.Append($"{i.Key} = {i.Value}{Environment.NewLine}");
				}
			}

			return result.ToString();
		}
	}
}
