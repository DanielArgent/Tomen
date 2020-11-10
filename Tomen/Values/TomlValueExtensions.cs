using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

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

		public static Boolean IsLocalDate(this TomlValue value) {
			return value is TomlLocalDate;
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

		public static List<TomlValue> AsList(this TomlValue value) {
			return (value as TomlArray).Value;
		}

		public static DateTime? AsDateTime(this TomlValue value) {
			if(value is TomlLocalDate localDate) {
				return new DateTime(localDate.Year, localDate.Month, localDate.Day);
			}

			if(value is TomlLocalDateTime dateTime) {
				return dateTime.Value;
			}

			if(value is TomlLocalTime localTime) {
				return new DateTime(1, 1, 1, localTime.Hours, localTime.Minutes, localTime.Seconds, localTime.Milliseconds);
			}

			return null;
		}

		public static DateTimeOffset? AsDateTimeOffset(this TomlValue value) {
			if (value is TomlDateTimeOffset dateTimeOffset) {
				return dateTimeOffset.Value;
			}

			if (value is TomlLocalDateTime dateTime) {
				return dateTime.Value;
			}

			if (value is TomlLocalDate localDate) {
				return new DateTime(localDate.Year, localDate.Month, localDate.Day);
			}

			if (value is TomlLocalTime localTime) {
				return new DateTime(1, 1, 1, localTime.Hours, localTime.Minutes, localTime.Seconds, localTime.Milliseconds);
			}


			return null;
		}

		public static TomlValue Path(this TomlTable value, String path) {
			TPath tpath = new TPathParser(new Lexer(path, "").GetTokens()).Parse();

			return tpath.Eval(value);
		}

		public static T Path<T>(this TomlTable value, String path) {
			var result = value.Path(path);

			try {
				if (typeof(T) == typeof(DateTime)) {
					return (T)Convert.ChangeType(result.AsDateTime(), typeof(T));
				}

				if (typeof(T) == typeof(DateTimeOffset)) {
					return (T)Convert.ChangeType(result.AsDateTimeOffset(), typeof(T));
				}

				if (typeof(T) == typeof(String)) {
					return (T)Convert.ChangeType(result.AsString(), typeof(T));
				}

				if (typeof(T) == typeof(Int32)) {
					return (T)Convert.ChangeType(result.AsInt(), typeof(T));
				}

				if (typeof(T) == typeof(Int64)) {
					return (T)Convert.ChangeType(result.AsInt(), typeof(T));
				}

				if (typeof(T) == typeof(Single)) {
					return (T)Convert.ChangeType(result.AsDouble(), typeof(T));
				}

				if (typeof(T) == typeof(Double)) {
					return (T)Convert.ChangeType(result.AsDouble(), typeof(T));
				}

				if (typeof(T) == typeof(Boolean)) {
					return (T)Convert.ChangeType(result.AsBoolean(), typeof(T));
				}

				if (typeof(T) == typeof(Int32[])) {
					return (T)Convert.ChangeType(result.AsList().Select(i => (Int32)i.AsInt()).ToArray(), typeof(T));
				}

				if (typeof(T) == typeof(Int64[])) {
					return (T)Convert.ChangeType(result.AsList().Select(i => (Int64)i.AsInt()).ToArray(), typeof(T));
				}

				if (typeof(T) == typeof(Single[])) {
					return (T)Convert.ChangeType(result.AsList().Select(i => (Single)i.AsDouble()).ToArray(), typeof(T));
				}

				if (typeof(T) == typeof(Double[])) {
					return (T)Convert.ChangeType(result.AsList().Select(i => i.AsDouble()).ToArray(), typeof(T));
				}

				if (typeof(T) == typeof(Boolean[])) {
					return (T)Convert.ChangeType(result.AsList().Select(i => i.AsBoolean()).ToArray(), typeof(T));
				}

				if (typeof(T) == typeof(String[])) {
					return (T)Convert.ChangeType(result.AsList().Select(i => i.AsString()).ToArray(), typeof(T));
				}

				if (typeof(T) == typeof(TomlValue[])) {
					return (T)Convert.ChangeType(result.AsList().ToArray(), typeof(T));
				}

				return (T)Convert.ChangeType(result, typeof(T));
			}
			catch (InvalidCastException) {
				return default(T);
			}
		}
	}
}
