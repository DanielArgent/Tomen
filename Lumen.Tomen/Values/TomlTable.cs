using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Tomen {
	public class TomlTable : ITomlValue {
		internal readonly Dictionary<String, ITomlValue> pairs;
		public String Name { get; }

		public ITomlValue this[String name] {
			get {
				foreach (KeyValuePair<String, ITomlValue> i in this.pairs) {
					if (i.Key == name) {
						return i.Value;
					}
				}
				return null;
			}
			set {
				this.pairs[name] = value;
			}
		}

		public TomlTable(String name) {
			this.Name = name;
			this.pairs = new Dictionary<String, ITomlValue>();
		}

		public Boolean Contains(String key) {
			foreach (KeyValuePair<String, ITomlValue> i in this.pairs) {
				if (i.Key == key) {
					return true;
				}
			}

			return false;
		}

		public override String ToString() {
			StringBuilder result = new StringBuilder();

			if (this.Name != null) {
				result.Append($"[{Lexer.NormalizeKey(this.Name)}]{Environment.NewLine}");
			}

			foreach (KeyValuePair<String, ITomlValue> i in this.pairs.OrderBy(i => i.Value is TomlTable ? 1 : 0)) {
				if (i.Value is TomlTable) {
					result.Append($"{Environment.NewLine}{i.Value}{Environment.NewLine}");
				}
				else {
					result.Append($"{Lexer.NormalizeKey(i.Key)} = {i.Value}{Environment.NewLine}");
				}
			}

			return result.ToString();
		}

	}
}
