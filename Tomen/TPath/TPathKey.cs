using System;
using System.Linq;

namespace Tomen {
	internal class TPathKey : TPath {
		private String key;
		private Int32 currentLine;

		public TPathKey(String key, System.Int32 currentLine) {
			this.key = key;
			this.currentLine = currentLine;
		}

		public TomlValue Eval(TomlValue value) {
			if(value is TomlTable table) {
				if (table.Contains(key)) {
					return table[key];
				}
				else {
					throw new TPathException($"table does not contains key '{this.key}'");
				}
			}

			if (value is TomlArray array) {
				return new TomlArray(array.Value.Select(i => this.Eval(i)).ToList());
			}

			throw new TPathException($"operation . does not supported with type {value.GetType()}");
		}
	}

	public class TPathException : Exception {
		public TPathException(String message) : base(message) {

		}
	}
}