using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Tomen {
	internal class TomlInlineTable : TomlTable {
		public override Boolean IsClosed => true;

		public TomlInlineTable() : base("") { }

		public override String ToString() {
			StringBuilder result = new StringBuilder("{ ");

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i => i.Value is TomlDottedTable)) {
				result
					.Append((i.Value as TomlDottedTable).ToStringInlined())
					.Append(", ");
			}

			foreach (KeyValuePair<String, TomlValue> i in this.pairs.Where(i => i.Value is not TomlDottedTable)) {
				result
					.Append(Lexer.NormalizeKey(i.Key)).Append(" = ").Append(i.Value.ToString())
					.Append(", ");
			}

			return result
				.Remove(result.Length - 2, 2)
				.Append(" }")
				.ToString();
		}
	}
}
