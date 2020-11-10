using System;
using System.Collections.Generic;
using System.Linq;

namespace Tomen {
	internal class TPathAllValues : TPath {
		public TomlValue Eval(TomlValue value) {
			if (value is TomlTable table) {
				return new TomlArray(table.pairs.Values.ToList());
			}
			else {
				List<TomlValue> result = new List<TomlValue>();
				foreach (var item in value.AsList()) {
					result.AddRange(this.Eval(item).AsList());
				}
				return new TomlArray(result);
			}
		}
	}
}