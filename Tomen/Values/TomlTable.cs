using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Tomen {
	public class TomlTable : TomlValue {
		internal readonly Dictionary<String, TomlValue> pairs;
		public String? Name { get; }

		public virtual Boolean IsClosed { get; set; }

		public TomlValue this[String name] {
			get {
				foreach (KeyValuePair<String, TomlValue> i in this.pairs) {
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

		public TomlTable(String? name) {
			this.Name = name;
			this.pairs = new Dictionary<String, TomlValue>();
		}

		public Boolean Contains(String key) {
			foreach (KeyValuePair<String, TomlValue> i in this.pairs) {
				if (i.Key == key) {
					return true;
				}
			}

			return false;
		}

		internal String ToString(String? prefix) {
			if (prefix is null) {
				return this.ToString();
			}

			StringBuilder result = new StringBuilder();

			String? newPrefix = this.Name == null ? null : prefix + '.' + this.Name;
			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i =>
				i.Value is TomlTable && i.Value is not TomlDottedTable && i.Value is not TomlInlineTable)) {

				result
					.Append((i.Value as TomlTable).ToString(newPrefix))
					.Append(Environment.NewLine);
			}

			if (this.Name != null) {
				result
					.Append('[').Append(prefix + '.' + Lexer.NormalizeKey(this.Name)).Append(']')
					.Append(Environment.NewLine);
			}

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i => i.Value is TomlInlineTable || i.Value is not TomlTable)) {
				result
					.Append(Lexer.NormalizeKey(i.Key)).Append(" = ").Append(i.Value.ToString())
					.Append(Environment.NewLine);
			}

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i => i.Value is TomlDottedTable)) {
				result
					.Append(i.Value.ToString())
					.Append(Environment.NewLine)
					.Append(Environment.NewLine);
			}

			return result.ToString();
		}

		public override String ToString() {
			StringBuilder result = new StringBuilder();


			if (this.Name != null) {
				foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i =>
					i.Value is TomlTable && i.Value is not TomlDottedTable && i.Value is not TomlInlineTable)) {

					result
						.Append((i.Value as TomlTable).ToString(this.Name))
						.Append(Environment.NewLine);
				}

				result
					.Append('[').Append(Lexer.NormalizeKey(this.Name)).Append(']')
					.Append(Environment.NewLine);
			}

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i => i.Value is TomlInlineTable || i.Value is not TomlTable)) {
				result
					.Append(Lexer.NormalizeKey(i.Key)).Append(" = ").Append(i.Value.ToString())
					.Append(Environment.NewLine);
			}

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i => i.Value is TomlDottedTable)) {
				result
					.Append(i.Value.ToString())
					.Append(Environment.NewLine)
					.Append(Environment.NewLine);
			}

			if (this.Name == null) {
				foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i =>
					i.Value is TomlTable && i.Value is not TomlDottedTable && i.Value is not TomlInlineTable)) {

					result
						.Append((i.Value as TomlTable).ToString(this.Name))
						.Append(Environment.NewLine);
				}
			}

				return result.ToString();
		}
	}
}
