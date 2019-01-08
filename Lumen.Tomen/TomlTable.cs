using System;
using System.Text;
using System.Collections.Generic;

namespace Lumen.Tomen {
	public class TomlTable : ITomlValue {
		private readonly Dictionary<ITomenKey, ITomlValue> pairs;
		public String Name { get => this.name?.Value; }
		private readonly ITomenKey name;

		public ITomlValue this[ITomenKey name] {
			get {
				foreach (KeyValuePair<ITomenKey, ITomlValue> i in this.pairs) {
					if (i.Key.Value == name.Value) {
						return i.Value;
					}
				}
				return null;
			}
			set {
				foreach (KeyValuePair<ITomenKey, ITomlValue> i in this.pairs) {
					if (i.Key.Value == name.Value) {
						this.pairs[i.Key] = value;
						return;
					}
				}

				this.pairs[name] = value;
			}
		}

		public ITomlValue this[String name] {
			get {
				foreach (KeyValuePair<ITomenKey, ITomlValue> i in this.pairs) {
					if (i.Key.Value == name) {
						return i.Value;
					}
				}
				return null;
			}
			set {
				if (Lexer.IsValidId(name)) {
					this.pairs[new TomlString(name)] = value;
				}
				else {
					this.pairs[new QuotedKey(name)] = value;
				}
			}
		}

		public TomlTable(String name) {
			if (name != null) {
				if (Lexer.IsValidId(name)) {
					this.name = new TomlString(name);
				}
				else {
					this.name = new QuotedKey(name);
				}
			}

			this.pairs = new Dictionary<ITomenKey, ITomlValue>();
		}

		public Boolean Contains(String key) {
			foreach (KeyValuePair<ITomenKey, ITomlValue> i in this.pairs) {
				if (i.Key.Value == key) {
					return true;
				}
			}

			return false;
		}

		public override String ToString() {
			StringBuilder result = new StringBuilder();

			if (this.name != null) {
				if (this.name is TomlString ts) {
					result.Append($"[{ts.Value}]{Environment.NewLine}");
				}
				else {
					result.Append($"[{this.name}]{Environment.NewLine}");
				}
			}

			foreach (KeyValuePair<ITomenKey, ITomlValue> i in this.pairs) {
				if (i.Value is TomlTable) {
					result.Append($"{Environment.NewLine}{i.Value}{Environment.NewLine}");
				}
				else {
					if (i.Key is TomlString) {
						result.Append($"{i.Key.Value} = {i.Value}{Environment.NewLine}");
					}
					else {
						result.Append($"{i.Key} = {i.Value}{Environment.NewLine}");
					}
				}
			}

			return result.ToString();
		}
	}
}
