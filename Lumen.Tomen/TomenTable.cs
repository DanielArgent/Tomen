using System;
using System.Text;
using System.Collections.Generic;

namespace Lumen.Tomen {
	public class TomenTable : ITomenValue {
		private readonly Dictionary<ITomenKey, ITomenValue> pairs;
		public String Name { get; }

		public ITomenValue this[ITomenKey name] {
			get {
				foreach (KeyValuePair<ITomenKey, ITomenValue> i in this.pairs) {
					if (i.Key.Value == name.Value) {
						return i.Value;
					}
				}
				return null;
			}
			set {
				foreach (KeyValuePair<ITomenKey, ITomenValue> i in this.pairs) {
					if (i.Key.Value == name.Value) {
						this.pairs[i.Key] = value;
						return;
					}
				}

				if (Lexer.IsValidId(name.Value)) {
					this.pairs[new TomenString(name.Value)] = value;
				}
				else {
					this.pairs[new QuotedKey(name.Value)] = value;
				}
			}
		}

		public ITomenValue this[String name] {
			get { 
				foreach(KeyValuePair<ITomenKey, ITomenValue> i in this.pairs) {
					if(i.Key.Value == name) {
						return i.Value;
					}
				}
				return null;
			}
			set {
				if (Lexer.IsValidId(name)) {
					this.pairs[new TomenString(name)] = value;
				}
				else {
					this.pairs[new QuotedKey(name)] = value;
				}
			}
		}

		public TomenTable(String name) {
			this.Name = name;
			this.pairs = new Dictionary<ITomenKey, ITomenValue>();
		}

		public Boolean Contains(String key) {
			foreach (KeyValuePair<ITomenKey, ITomenValue> i in this.pairs) {
				if (i.Key.Value == key) {
					return true;
				}
			}

			return false;
		}

		public override String ToString() {
			StringBuilder result = new StringBuilder();

			if (this.Name != null) {
				result.Append($"[{this.Name}]{Environment.NewLine}");
			}

			foreach (KeyValuePair<ITomenKey, ITomenValue> i in this.pairs) {
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
