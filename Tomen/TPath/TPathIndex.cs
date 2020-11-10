using System;
using System.Linq;

namespace Tomen {
	internal class TPathIndex : TPath {
		private TPath result;
		private Int32 index;

		public TPathIndex(TPath result, Int32 index) {
			this.result = result;
			this.index = index;
		}

		public TomlValue Eval(TomlValue table) {
			var tomlValue = result?.Eval(table) ?? table;

			if (tomlValue is TomlTable tomlTable) {
				return tomlTable.pairs.ElementAt(index).Value;
			}
			else if (tomlValue is TomlArray tomlArray) {
				return tomlArray.Value[index];
			}

			throw new TPathException("indexation only support arrays and tables");
		}
	}
}