using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Tomen {
	public static class TomlValueExtensions {
		public static Boolean IsBool(this TomlValue value) {
			return value is TomlBool;
		}

		public static Boolean IsInt(this TomlValue value) {
			return value is TomlInt;
		}

		public static Boolean IsDouble(this TomlValue value) {
			return value is TomlDouble;
		}

		public static Boolean IsString(this TomlValue value) {
			return value is TomlString;
		}

		public static Boolean IsArray(this TomlValue value) {
			return value is TomlArray;
		}

		public static Boolean IsTable(this TomlValue value) {
			return value is TomlTable;
		}

		public static Boolean? AsBoolean(this TomlValue value) {
			if (value is TomlBool tomlInt) {
				return tomlInt.Value;
			}

			return null;
		}

		public static Int64? AsInt(this TomlValue value) {
			if (value is TomlInt tomlInt) {
				return tomlInt.Value;
			}

			return null;
		}

		public static Double? AsDouble(this TomlValue value) {
			if (value is TomlDouble tomlDouble) {
				return tomlDouble.Value;
			}

			return null;
		}

		public static String AsString(this TomlValue value) {
			if (value is TomlString tomlString) {
				return tomlString.Value;
			}

			return null;
		}

		public static TomlTable AsTable(this TomlValue value) {
			return value as TomlTable;
		}

		public static TomlValue Path(this TomlTable value, String path) {
			String[] subPaths = path.Split('.');

			for (var i = 0; i < subPaths.Length - 1; i++) {
				if (value == null) {
					return null;
				}

				value = value[subPaths[i]].AsTable();
			}

			return value[subPaths[subPaths.Length - 1]];
		}

		public static T Path<T>(this TomlTable value, String path) {
			String[] subPaths = path.Split('.');

			for (var i = 0; i < subPaths.Length - 1; i++) {
				if (value == null) {
					return default(T);
				}

				value = value[subPaths[i]].AsTable();
			}

			var result = value[subPaths[subPaths.Length - 1]];

			if(typeof(T) == typeof(String)) {
				try {
					return (T)Convert.ChangeType(result.AsString(), typeof(T));
				}
				catch (InvalidCastException) {
					return default(T);
				}
			}

			if (typeof(T) == typeof(Int32)) {
				try {
					return (T)Convert.ChangeType(result.AsInt(), typeof(T));
				}
				catch (InvalidCastException) {
					return default(T);
				}
			}

			if (typeof(T) == typeof(Int64)) {
				try {
					return (T)Convert.ChangeType(result.AsInt(), typeof(T));
				}
				catch (InvalidCastException) {
					return default(T);
				}
			}

			if (typeof(T) == typeof(float)) {
				try {
					return (T)Convert.ChangeType(result.AsDouble(), typeof(T));
				}
				catch (InvalidCastException) {
					return default(T);
				}
			}

			if (typeof(T) == typeof(Double)) {
				try {
					return (T)Convert.ChangeType(result.AsDouble(), typeof(T));
				}
				catch (InvalidCastException) {
					return default(T);
				}
			}

			if (typeof(T) == typeof(Boolean)) {
				try {
					return (T)Convert.ChangeType(result.AsBoolean(), typeof(T));
				}
				catch (InvalidCastException) {
					return default(T);
				}
			}

			try {
				return (T)Convert.ChangeType(result, typeof(T));
			}
			catch (InvalidCastException) {
				return default(T);
			}
		}

	}
}
