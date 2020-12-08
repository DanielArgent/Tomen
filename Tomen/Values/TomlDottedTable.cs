using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Tomen {
	internal class TomlDottedTable : TomlTable {
		public override Boolean IsClosed => parent.IsClosed;

		public TomlTable parent;

		public TomlDottedTable(TomlTable parent, String name) : base(name) {
			this.parent = parent;
		}

		internal String ToStringInlined() {
			StringBuilder result = new StringBuilder();

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i =>
				i.Value is TomlInlineTable || i.Value is not TomlTable)) {

				result
					.Append(Lexer.NormalizeKey(this.Name)).Append('.').Append(Lexer.NormalizeKey(i.Key))
					.Append(" = ").Append(i.Value.ToString())
					.Append(", ");
			}

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i => i.Value is TomlDottedTable)) {
				result
					.Append(Lexer.NormalizeKey(this.Name)).Append('.').Append(i.Value.ToString())
					.Append(", ");
			}

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i => i.Value is not TomlDottedTable && i.Value is not TomlInlineTable && i.Value is TomlTable)) {
				String inlineTableRepr = (i.Value as TomlTable).ToString(BuildRecursivePrefix(this));
				result
					.Append(inlineTableRepr)
					.Append(", ");
			}

			return result.Remove(result.Length - 2, 2).ToString();
		}

		public override String ToString() {
			StringBuilder result = new StringBuilder();

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i =>
				i.Value is TomlInlineTable || i.Value is not TomlTable)) {

				result
					.Append(Lexer.NormalizeKey(this.Name)).Append('.').Append(Lexer.NormalizeKey(i.Key))
					.Append(" = ").Append(i.Value.ToString())
					.Append(Environment.NewLine);
			}

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i =>
				i.Value is TomlDottedTable)) {

				result
					.Append(Lexer.NormalizeKey(this.Name)).Append('.').Append(i.Value.ToString())
					.Append(Environment.NewLine);
			}

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i => i.Value is not TomlDottedTable && i.Value is not TomlInlineTable && i.Value is TomlTable)) {
				String inlineTableRepr = (i.Value as TomlTable).ToString(BuildRecursivePrefix(this));
				result
					.Append(inlineTableRepr)
					.Append(Environment.NewLine)
					.Append(Environment.NewLine);
			}

			return result.ToString();
		}

		private static String BuildRecursivePrefix(TomlDottedTable dottedTable) {
			if (dottedTable.parent is TomlDottedTable dottedTable2) {
				return BuildRecursivePrefix(dottedTable2) + '.' + Lexer.NormalizeKey(dottedTable.Name);
			}
			else {
				return Lexer.NormalizeKey(dottedTable.parent.Name) + '.' + Lexer.NormalizeKey(dottedTable.Name);
			}
		}
	}
}
