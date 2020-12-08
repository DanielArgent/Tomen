using System;
using System.Text;
using System.Collections.Generic;

namespace Tomen {
	internal class TomlArrayOfTables : TomlArray {
		public String Name { get; set; }

		public TomlArrayOfTables(String name, List<TomlValue> value) : base(value) {
			this.Name = name;
		}

		public override String ToString() {
			StringBuilder builder = new StringBuilder();

			foreach (TomlValue value in this.Value) {
				TomlTable table = value as TomlTable;

				builder.Append($"[[{this.Name}]]");

			}

			return builder.ToString();
		}
	}
}
